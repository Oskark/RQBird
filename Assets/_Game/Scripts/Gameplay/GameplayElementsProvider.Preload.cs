
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gameplay.Levels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Gameplay
{
	using LevelSegmentRef = AssetReferenceGameObject;

	public partial class GameplayElementsProvider
	{
		private Dictionary<LevelSegmentRef, LevelSegment> _preloadedSegments;

		private bool _alreadyPreloaded = false;
		
		
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
		
				
		private void InitContainers()
		{
			_preloadedSegments = new Dictionary<LevelSegmentRef, LevelSegment>();
			_segmentsPool = new Dictionary<LevelSegmentRef, IObjectPool<LevelSegment>>();
			_poolContainer = new GameObject( "SegmentsPool" ).transform;
			
			GameObject.DontDestroyOnLoad( _poolContainer );
		}

		
		private async Task PreloadAllElements()
		{
			var allTasks = new List<UniTask>();

			
			var floorAR = _elementsContainerInstance.FloorSegmentAR;
			
			var  noFloorPreloadedYet = _preloadedSegments.TryGetValue( floorAR, out var segment ) == false || segment == null;
			if ( noFloorPreloadedYet )
			{
				allTasks.Add( PreloadElementAsync( floorAR ) );
			}
			
			foreach ( var mapSegment in _elementsContainerInstance.LevelSegmentsAR )
			{
				var  wasNotPreloadedYet = _preloadedSegments.TryGetValue( mapSegment, out var value ) == false || value == null;
				if ( wasNotPreloadedYet )
				{
					allTasks.Add( PreloadElementAsync( mapSegment ) );
				}
			}
            
			if ( allTasks.Count > 0 )
				await UniTask.WhenAll( allTasks );


			_alreadyPreloaded = true;
		}
		
		private async UniTask PreloadElementAsync(LevelSegmentRef @ref )
		{
			_preloadedSegments.TryAdd( @ref, null );
			
			// Debug.Log($"Started preloading element: {@ref} "  );

			var handle =  @ref.LoadAssetAsync<GameObject>();
			await handle.Task;
            
			// Debug.Log($"Element {@ref} preloaded "  );

			var success = handle.Status == AsyncOperationStatus.Succeeded;
			var segment = handle.Result;

			// Addressables.Release( handle );
			
			if ( success )
			{
				var levelSegment = segment.GetComponent<LevelSegment>();
				_preloadedSegments[@ref] = levelSegment;

				_segmentsPool.TryAdd( @ref, new ObjectPool<LevelSegment>
				(
					createFunc: () => CreateSegmentForPool(@ref),
					actionOnGet: GetSegmentFromPool,
					actionOnRelease: ReturnSegmentToPool,
					actionOnDestroy: DestroySegmentFromPool,
					defaultCapacity: _gameplayConfig.EntryPoolDefaultCapacity,
					maxSize: _gameplayConfig.EntryPoolMaxCapacity
				));
			}
			else
			{
				Debug.LogError( $"Failed to load element {@ref}" );
			}

			// Debug.Log($"Element {@ref} finished "  );
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

		private void ClearPreloaded()
		{
			foreach ( var preloadedElementsValue in _preloadedSegments.Values )
			{
				if (preloadedElementsValue != null)
					Addressables.ReleaseInstance( preloadedElementsValue.gameObject );
			}

			_preloadedSegments.Clear();
			
			_alreadyPreloaded = false;
        }

	}
}
