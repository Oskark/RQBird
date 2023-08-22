using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gameplay.Levels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
// ReSharper disable ClassNeverInstantiated.Global


namespace Gameplay
{
	using LevelSegmentRef = AssetReferenceGameObject;

	public class GameplayElementsProvider
	{
		[Inject(Id = "ElementsContainerRef")] 
		private AssetReference _elementsContainerRef;

		[Inject] private GameplayData _gameplayData;
		
		private GameplayElementsContainer _elementsContainerInstance;
		
		private Dictionary<LevelSegmentRef, LevelSegment> _preloadedSegments;
		private Dictionary<LevelSegmentRef, IObjectPool<LevelSegment>> _segmentsPool;

		private Transform _poolContainer;
        
		// Static to prevent double preloading
		private static bool _alreadyPreloaded = false;


		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		private static void ResetStatics()
		{
			_alreadyPreloaded = false;
		}
		
		public LevelSegment GetFloor()
		{
			var elem = _elementsContainerInstance.FloorSegmentAR;
			return GetSegment( elem );
		}

		public LevelSegment GetRandomSegment()
		{
			var elem = _elementsContainerInstance.LevelSegmentsAR.RandomElement();
			return GetSegment( elem );
		}
		

		
		public void ReturnSegment( LevelSegment segment )
		{
			var segmentRef = segment.OriginAssetRef;
			
			_segmentsPool[segmentRef].Release( segment );
		}

		public async Task PreloadElements()
		{
            if (_alreadyPreloaded) return;
			
			InitContainers();

			var noElementsContainer = _elementsContainerInstance == null;
			if (noElementsContainer)
			{
				var (success, container) = await WaitForElementsContainer();
				if ( success == false )
				{
					Debug.LogError( "Failed to preload elements" );
					return;
				}

				_elementsContainerInstance = container;
			}

			await PreloadAllElements();
		}
		
		public void Clear()
		{
			_segmentsPool.Clear();

			_elementsContainerInstance = null;
			
			foreach ( var preloadedElementsValue in _preloadedSegments.Values )
			{
				Addressables.ReleaseInstance( preloadedElementsValue.gameObject );
			}
			
			if (_poolContainer != null)
				GameObject.Destroy( _poolContainer.gameObject );
			
			
			_preloadedSegments.Clear();
			_alreadyPreloaded = false;
		}

		public (float left, float right) GetFloorLeftRightSegment()
		{
			return _preloadedSegments[_elementsContainerInstance.FloorSegmentAR].GetLeftRightBounds();
		}

		private async Task PreloadAllElements()
		{
			var allTasks = new List<UniTask>();

			var floorAR = _elementsContainerInstance.FloorSegmentAR;
			var noFloorPreloadedYet = _preloadedSegments.TryGetValue( floorAR, out var segment ) == false || segment == null;
			
			if ( noFloorPreloadedYet )
				allTasks.Add( PreloadElementAsync( floorAR ) );

			
			foreach ( var mapSegment in _elementsContainerInstance.LevelSegmentsAR )
			{
				var  wasNotPreloadedYet = _preloadedSegments.TryGetValue( mapSegment, out var value ) == false || value == null;
				if ( wasNotPreloadedYet )
				{
					allTasks.Add( PreloadElementAsync( mapSegment ) );
				}
			}

			Debug.Log( "All tasks awaited!"  );

			if ( allTasks.Count > 0 )
				await UniTask.WhenAll( allTasks );

			_alreadyPreloaded = true;
			Debug.Log( "All tasks completed!"  );
		}



		private async Task<AsyncOperationHandle<GameplayElementsContainer>> LoadElementsContainer()
		{
			var elementsContainerHandle = _elementsContainerRef.LoadAssetAsync<GameplayElementsContainer>();
			await elementsContainerHandle;
			
			return elementsContainerHandle;
		}
		
		private async Task<(bool, GameplayElementsContainer)> WaitForElementsContainer()
		{
			var elementsContainerHandle = await LoadElementsContainer();
			if ( elementsContainerHandle.Status != AsyncOperationStatus.Succeeded )
			{
				return (false, null);
			}

			var instance = elementsContainerHandle.Result;

			Addressables.Release( elementsContainerHandle );
			
			return (true, instance);
		}

		private void InitContainers()
		{
			_preloadedSegments = new Dictionary<LevelSegmentRef, LevelSegment>();
			_segmentsPool = new Dictionary<LevelSegmentRef, IObjectPool<LevelSegment>>();
			_poolContainer = new GameObject( "SegmentsPool" ).transform;
			GameObject.DontDestroyOnLoad( _poolContainer );
		}


		private async UniTask PreloadElementAsync(LevelSegmentRef @ref )
		{
			_preloadedSegments.TryAdd( @ref, null );
			Debug.Log($"Started preloading element: {@ref} "  );

			var handle =  @ref.LoadAssetAsync<GameObject>();
			await handle.Task;
			
			Debug.Log($"Element {@ref} preloaded "  );

			var success = handle.Status == AsyncOperationStatus.Succeeded;
			var segment = handle.Result;

			// Addressables.Release( handle );
			if ( success )
			{
				var levelSegment = segment.GetComponent<LevelSegment>();
				_preloadedSegments[@ref] = levelSegment;

				_segmentsPool.TryAdd( @ref, new ObjectPool<LevelSegment>
				(
					createFunc: CreateSegmentForPool,
					actionOnGet: OnGetFromPool,
					actionOnRelease: OnReturnToPool,
					actionOnDestroy: OnDestroyFromPool,
					defaultCapacity: _gameplayData.EntryPoolDefaultCapacity,
					maxSize: _gameplayData.EntryPoolMaxCapacity
				));
			}
			else
			{
				Debug.LogError( $"Failed to load element {@ref}" );
			}

			Debug.Log($"Element {@ref} finished "  );

			return;
			
			
			LevelSegment CreateSegmentForPool()
			{
				var instance = GameInstaller.SpawnStatic( _preloadedSegments[@ref].gameObject ).GetComponent<LevelSegment>();
				instance.transform.SetParent( _poolContainer );
				instance.gameObject.SetActive( false );

				instance.SetMyAssetRef( @ref );
				return instance;
			}

			void OnGetFromPool( LevelSegment segment )
			{
				segment.gameObject.SetActive( true );
				segment.Activate();
			}

			void OnReturnToPool( LevelSegment segment )
			{
				segment.ResetData();
				segment.transform.SetParent( _poolContainer );
				segment.gameObject.SetActive( false );
			}

			void OnDestroyFromPool( LevelSegment segment )
			{
				GameObject.Destroy( segment.gameObject );
			}
		}
        
		private LevelSegment GetSegment( LevelSegmentRef segmentRef )
		{
			return _segmentsPool[segmentRef].Get();
		}
	}
}
