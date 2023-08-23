
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Levels
{
	public class LevelSegment : MonoBehaviour
	{
        
		[SerializeField] private float _Speed = 8f;

		public AssetReferenceGameObject OriginAssetRef { get; private set; }
		[field: SerializeField] public Vector3 SpawnOffset { get; private set; }

		[SerializeField] private BoxCollider _Collider;
		[SerializeField] private float _DistanceRemovalThreshold = 10f;

		[Inject] private LevelManager _levelManager;

		private bool _isPaused = false;

		private Action<LevelSegment> _onElementDestroyed;
		private SignalBus _signalBus;

		[Inject]
		public void Construct( LevelManager levelManager, SignalBus signalBus )
		{
			_levelManager = levelManager;
			_signalBus = signalBus;
			
			Debug.Log($"Construct {name} {GetInstanceID()}"  );
		}

		private void OnGameplayStateChanged( GameplayStateChangedSignal newState )
		{
			var canMove = newState.CurrentState == GameState.Play;

			SetPause (canMove == false);
		}


		private void Start()
		{
			if ( _levelManager == null )
			{
				Debug.LogError( $"No levelMAnager on {name}" );
			}
		}

		public void Init( Action<LevelSegment> onElementDestroyed )
		{
			_onElementDestroyed = onElementDestroyed;
		}

		public void SetMyAssetRef( AssetReferenceGameObject asset )
		{
			OriginAssetRef = asset;
		}

		public void ResetData()
		{
			Debug.Log($"Reset data on {name} {GetInstanceID()}"  );
			_isPaused = false;
			_onElementDestroyed = null;
			
			_signalBus.Unsubscribe<GameplayStateChangedSignal>( OnGameplayStateChanged );
		}

		private void OnDestroy()
		{
			Debug.Log( $"OnDestroy on {name} {GetInstanceID()}" );
		}

		private void Update()
		{
			if ( _isPaused ) return;

			if ( _levelManager == null )
			{
				return;
			}
			
			transform.position += Vector3.back * (Time.deltaTime * _Speed * _levelManager.CurrentSpeed);

			if ( transform.position.z < -_DistanceRemovalThreshold )
			{
				_onElementDestroyed?.Invoke(this);
			}
		}


		public float GetZLength()
		{
			return _Collider.bounds.size.z;
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

		public void Activate()
		{
			_signalBus.Subscribe<GameplayStateChangedSignal>( OnGameplayStateChanged );
        }
	}
}
