﻿
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Levels
{
	public class LevelSegment : MonoBehaviour
	{

		public static event Action OnPlayerHitSegment;
		
		[SerializeField] private float _Speed = 8f;

		[field: SerializeField] public Vector3 SpawnOffset { get; private set; }
		
		[SerializeField] private Collider _Collider;
		[SerializeField] private float _DistanceRemovalThreshold = 10f;

		[Inject] private LevelManager _levelManager;
		[Inject] private GameInstaller _gameInstaller;

		private bool _isPaused = false;
		public AssetReferenceGameObject OriginAssetRef { get; private set; }

		private Action<LevelSegment> _onElementDestroyed;
		private SignalBus _signalBus;

		[Inject]
		public void Construct( LevelManager levelManager, SignalBus signalBus )
		{
			_levelManager = levelManager;
			_signalBus = signalBus;
            
			Debug.Log($"Construct on {GetInstanceID()}"  );
			
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
			
			if ( _gameInstaller == null )
			{
				Debug.LogError( $"No _gameInstaller on {name}" );
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
			_isPaused = false;
			_onElementDestroyed = null;
			
			_signalBus.TryUnsubscribe<GameplayStateChangedSignal>( OnGameplayStateChanged );
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

		private void OnCollisionEnter( Collision other )
		{
			if ( other.gameObject.CompareTag( "Player" ) )
			{
				OnPlayerHitSegment?.Invoke();
			}
		}

		public float GetZLength()
		{
			return _Collider.bounds.size.z;
		}


		public void SetPause( bool isPaused )
		{
			_isPaused = isPaused;
		}

		public (float left, float right) GetLeftRightBounds()
		{
			var bc = _Collider as BoxCollider;
			return (bc.bounds.min.x + 0.55f, bc.bounds.max.x - 0.5f); // TODO: take this value from proper place - player collider
		}

		public void Activate()
		{
			_signalBus.Subscribe<GameplayStateChangedSignal>( OnGameplayStateChanged );
		}
	}
}
