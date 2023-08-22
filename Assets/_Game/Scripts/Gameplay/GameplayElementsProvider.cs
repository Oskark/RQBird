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


namespace Gameplay
{
	using LevelSegmentRef = AssetReferenceGameObject;

	public class GameplayElementsProvider : IInitializable,IDisposable
	{
		[Inject(Id = "ElementsContainerRef")] 
		private AssetReference _ElementsContainerRef;
		private GameplayElementsContainer _elementsContainerInstance;
		
		private Dictionary<LevelSegmentRef, LevelSegment> _preloadedElements;
		private Dictionary<LevelSegmentRef, IObjectPool<LevelSegment>> _segmentsPool;

		private Transform _poolContainer;
        
		private static bool _alreadyPreloaded = false;


		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		private static void ResetStatics()
		{
			_alreadyPreloaded = false;
		}

		public void Initialize()
		{
		}
		
		[Inject]
		public void Construct( SignalBus signalBus )
		{
		}

		public void Dispose()
		{
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
			// Debug.Log( $"Preload on instance {GetInstanceID()}" );
			if (_alreadyPreloaded) return;
			
			_preloadedElements = new Dictionary<LevelSegmentRef, LevelSegment>();
			_segmentsPool = new Dictionary<LevelSegmentRef, IObjectPool<LevelSegment>>();
			_poolContainer = new GameObject("SegmentsPool").transform;
			GameObject.DontDestroyOnLoad( _poolContainer );
			
			if (_elementsContainerInstance == null)
			{
				var elementsContainerHandle = _ElementsContainerRef.LoadAssetAsync<GameplayElementsContainer>();
				await elementsContainerHandle;

				if ( elementsContainerHandle.Status != AsyncOperationStatus.Succeeded ) return;
				
				_elementsContainerInstance = elementsContainerHandle.Result as GameplayElementsContainer;
				
				Addressables.Release( elementsContainerHandle );
			}

			var allTasks = new List<UniTask>();

			if ( _preloadedElements.TryGetValue( _elementsContainerInstance.FloorSegmentAR, out var segment ) == false || segment == null) 
				allTasks.Add( PreloadElement( _elementsContainerInstance.FloorSegmentAR ) );
				
			foreach ( var t in _elementsContainerInstance.LevelSegmentsAR )
			{
				if ( _preloadedElements.TryGetValue( t, out var value ) == false || value == null)
				{
					allTasks.Add( PreloadElement( t ) );
				}
			}
			
			Debug.Log("All tasks awaited!"  );

			if (allTasks.Count > 0)
				await UniTask.WhenAll( allTasks);

			_alreadyPreloaded = true;
			Debug.Log("All tasks completed!"  );
			
		}
		

		private async UniTask PreloadElement(LevelSegmentRef _ref )
		{
			_preloadedElements.TryAdd( _ref, null );
			Debug.Log($"Started preloading element: {_ref} "  );

			var handle =  _ref.LoadAssetAsync<GameObject>();
			await handle.Task;
			
			Debug.Log($"Element {_ref} preloaded "  );

			var success = handle.Status == AsyncOperationStatus.Succeeded;
			var segment = handle.Result;

			Addressables.Release( handle );
			if ( success )
			{
				var levelSegment = segment.GetComponent<LevelSegment>();
				_preloadedElements[_ref] = levelSegment;

				_segmentsPool.TryAdd( _ref, new ObjectPool<LevelSegment>
				(
					createFunc: () =>
					{
						var instance = GameInstaller.SpawnStatic( _preloadedElements[_ref].gameObject ).GetComponent<LevelSegment>();
						instance.transform.SetParent( _poolContainer );
						instance.gameObject.SetActive( false );

						instance.SetMyAssetRef( _ref );
						return instance;
					},
					actionOnGet: segment =>
					{
						segment.gameObject.SetActive( true );
					},
					actionOnRelease: segment =>
					{
						segment.ResetData();
						segment.transform.SetParent( _poolContainer );
						segment.gameObject.SetActive( false );
						
					},
					actionOnDestroy: segment =>
					{
						GameObject.Destroy( segment.gameObject );
						
					},
					defaultCapacity: 3,
					maxSize: 5
				));
			}
			else
			{
				Debug.LogError( $"Failed to load element {_ref}" );
			}

			Debug.Log($"Element {_ref} finished "  );

			return;
		}
        
		private LevelSegment GetSegment( LevelSegmentRef segmentRef )
		{
			return _segmentsPool[segmentRef].Get();
		}

		public void Clear()
		{
			_segmentsPool.Clear();

			_elementsContainerInstance = null;
			
			foreach ( var preloadedElementsValue in _preloadedElements.Values )
			{
				Addressables.ReleaseInstance( preloadedElementsValue.gameObject );
			}
			
			if (_poolContainer != null)
				GameObject.Destroy( _poolContainer.gameObject );
			
			
			_preloadedElements.Clear();
			_alreadyPreloaded = false;
		}

		public (float left, float right) GetFloorLeftRightSegment()
		{
			return _preloadedElements[_elementsContainerInstance.FloorSegmentAR].GetLeftRightBounds();
		}
	}
}
