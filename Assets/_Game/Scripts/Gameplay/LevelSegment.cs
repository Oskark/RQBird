
using System;
using UnityEngine;

namespace Gameplay.Levels
{
	public class LevelSegment : MonoBehaviour
	{

		public static event Action OnPlayerHitSegment;
		
		[SerializeField] private float _Speed = 8f;

		[field: SerializeField] public Vector3 SpawnOffset { get; private set; }
		
		[SerializeField] private Collider _Collider;
		[SerializeField] private float _DistanceRemovalThreshold = 10f;

		private LevelManager _levelManager;
		private bool _isPaused = false;
		
		private Action<LevelSegment> _onElementDestroyed;

		// [Inject]
		public void Construct( LevelManager levelManagerable )
		{
			_levelManager = levelManagerable;
		}
		
		public void Init( LevelManager levelManager, Action<LevelSegment> onElementDestroyed )
		{
			_levelManager = levelManager;
			_onElementDestroyed = onElementDestroyed;
		}
		
		private void Update()
		{
			if ( _isPaused ) return;
			
			transform.position += Vector3.back * (Time.deltaTime * _Speed * _levelManager.SpeedModifier);

			if ( transform.position.z < -_DistanceRemovalThreshold )
			{
				_onElementDestroyed?.Invoke(this);
				
				Destroy( gameObject );
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
	}
}
