using System;
using System.Collections.Generic;
using Gameplay.Levels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using Zenject;
// ReSharper disable ClassNeverInstantiated.Global


namespace Gameplay
{
	using LevelSegmentRef = AssetReferenceGameObject;

	public partial class GameplayElementsProvider : IDisposable
	{
		[Inject(Id = "ElementsContainerRef")] 
		private AssetReference _elementsContainerRef;

		[Inject] private GameplayData _gameplayData;
		
		private GameplayElementsContainer _elementsContainerInstance;
		private Dictionary<LevelSegmentRef, IObjectPool<LevelSegment>> _segmentsPool;

		private Transform _poolContainer;
		
		private SignalBus _signalBus;

		[Inject]
		private void Construct(SignalBus signalBus)
		{
			_signalBus = signalBus;
			_signalBus.Subscribe<ExitGameplaySignal>( OnExitFromGameplay );
		}

		private void OnExitFromGameplay()
		{
			Clear();
		}

		public (float left, float right) GetFloorLeftRightSegment()
		{
			return _preloadedSegments[_elementsContainerInstance.FloorSegmentAR].GetLeftRightBounds();
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


		private LevelSegment GetSegment( LevelSegmentRef segmentRef )
		{
			return _segmentsPool[segmentRef].Get();
		}
		
		
		private LevelSegment CreateSegmentForPool( AssetReferenceGameObject @ref )
		{
			var instance = GameInstaller.SpawnStatic( _preloadedSegments[@ref].gameObject ).GetComponent<LevelSegment>();
			instance.transform.SetParent( _poolContainer );
			instance.gameObject.SetActive( false );

			instance.SetMyAssetRef( @ref );
			return instance;
		}

		private void GetSegmentFromPool( LevelSegment segment )
		{
			segment.gameObject.SetActive( true );
			segment.Activate();
		}

		private void ReturnSegmentToPool( LevelSegment segment )
		{
			segment.ResetData();
			segment.transform.SetParent( _poolContainer );
			segment.gameObject.SetActive( false );
		}

		private void DestroySegmentFromPool( LevelSegment segment )
		{
			GameObject.Destroy( segment.gameObject );
		}


		

		public void Dispose()
		{
			Debug.Log( $"{GetType()}: Dispose" );
			Clear();
			
			_signalBus?.Unsubscribe<ExitGameplaySignal>( OnExitFromGameplay );
		}
		
		public void Clear()
		{
			foreach ( var kvp in _segmentsPool )
			{
				kvp.Value.Clear();
			}
            
			_elementsContainerInstance = null;

			
			if ( _poolContainer != null )
			{
				GameObject.Destroy( _poolContainer.gameObject );
			}
			
			ClearPreloaded();
        }
	}
}
