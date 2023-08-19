
using System;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Gameplay.Levels
{
	public class LevelSegment : MonoBehaviour
	{

		[SerializeField] private float _Speed = 8f;

		[field: SerializeField] public Vector3 SpawnOffset { get; private set; }
		
		[SerializeField] private Collider _Collider;
		[SerializeField] private float _DistanceRemovalThreshold = 10f;

		private LevelManager _levelManager;
		
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
			transform.position += Vector3.back * (Time.deltaTime * _Speed * _levelManager.SpeedModifier);

			if ( transform.position.z < -_DistanceRemovalThreshold )
			{
				_onElementDestroyed?.Invoke(this);
				
				Destroy( gameObject );
			}
		}

		public float GetZLength()
		{
			return _Collider.bounds.size.z;
		}


	}
}
