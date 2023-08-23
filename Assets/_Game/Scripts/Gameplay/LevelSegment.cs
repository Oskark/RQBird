
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Levels
{
	public class LevelSegment : MonoBehaviour
	{
		public AssetReferenceGameObject OriginAssetRef { get; private set; }
		[field: SerializeField] public Vector3 SpawnOffset { get; private set; }

		[SerializeField] private BoxCollider _Collider;
		[SerializeField] private float _DistanceRemovalThreshold = 10f;

		[Inject] private LevelManager _levelManager;

		private bool _isPaused = false;

		private Action<LevelSegment> _onElementPassedPlayer;
		private SignalBus _signalBus;

		[Inject]
		public void Construct( LevelManager levelManager, SignalBus signalBus )
		{
			_levelManager = levelManager;
			_signalBus = signalBus;
        }
		
		public void Activate()
		{
			_signalBus.Subscribe<GameplayStateChangedSignal>( OnGameplayStateChanged );
		}

		public void Init( Action<LevelSegment> onElementDestroyed )
		{
			_onElementPassedPlayer = onElementDestroyed;
		}

		public void SetMyAssetRef( AssetReferenceGameObject asset )
		{
			OriginAssetRef = asset;
		}

		public void ResetData()
		{
			_isPaused = false;
			_onElementPassedPlayer = null;
			
			_signalBus.Unsubscribe<GameplayStateChangedSignal>( OnGameplayStateChanged );
		}

		private void Update()
		{
			if ( _isPaused ) return;

			var  noLevelManager = _levelManager == null;
			if ( noLevelManager )
			{
				return;
			}
			
			MoveSegment();

			if ( AlreadyPassedDistanceRemovalThreshold() )
			{
				ReportElementPassed();
			}
		}

		public float GetZLength()
		{
			return _Collider.bounds.size.z;
		}



		private void MoveSegment()
		{
			transform.position += Vector3.back * (Time.deltaTime * _levelManager.CurrentSpeed);
		}
		
		private bool AlreadyPassedDistanceRemovalThreshold()
		{
			return transform.position.z < -_DistanceRemovalThreshold;
		}
		
		private void ReportElementPassed()
		{
			_onElementPassedPlayer?.Invoke( this );
		}

        
		private void OnGameplayStateChanged( GameplayStateChangedSignal newState )
		{
			var canMove = newState.CurrentState == GameState.Play;

			SetPause (canMove == false);
		}
        
		private void SetPause( bool isPaused )
		{
			_isPaused = isPaused;
		}

		public (float left, float right) GetLeftRightBounds()
		{
			var bc = _Collider;
			var mostLeftPosition = bc.bounds.min.x;
			var mostRightPosition = bc.bounds.max.x;

			var currentXPosition = transform.position.x;
			var mostLeftWorldPosition  = currentXPosition + mostLeftPosition;
			var mostRightWorldPosition = currentXPosition + mostRightPosition;
			
			return (mostLeftWorldPosition, mostRightWorldPosition);
		}

	
	}
}
