using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Levels
{
	public class LevelGenerator : MonoBehaviour
	{
		
		[SerializeField] private LevelManager _LevelManager;
        
		[SerializeField] private bool _isPaused = false;
		
		[SerializeField] private LevelSegment[] _LevelSegments;
		[SerializeField] private LevelSegment _FloorSegment;

		private float _floorRepeatRange;
		
        
		private Transform _PlayerTransform;
		private float _distancePassed = 0;
		
		private List<LevelSegment> _spawnedFloorSegments = new List<LevelSegment>();
		private List<LevelSegment> _spawnedObstacles = new List<LevelSegment>();

		private Action _onObstaclePassed;

		private void Awake()
		{
			_floorRepeatRange = _FloorSegment.GetZLength();
		}

		private void Start()
		{
			SpawnFloors(  10 );
			SpawnObstacles( 10 );
		}

		void Update()
		{
			if (_isPaused) return;
        }

		public void UpdateCurrentDistance( float distancePassed )
		{
			_distancePassed = distancePassed;
		}

		#region Floors

		private void SpawnFloors( int amount )
		{
			for ( int i = 0; i < amount; i++ )
			{
				SpawnNewFloorSegmentAtTheEnd();
			}
		}

		private void SpawnNewFloorSegmentAtTheEnd()
		{
			var lastFloor = _spawnedFloorSegments.Count == 0 ? null : _spawnedFloorSegments[^1];

			var newSpawnPosition = lastFloor == null ? Vector3.zero : lastFloor.transform.position + Vector3.forward * lastFloor.GetZLength();
				
			SpawnFloorAt(newSpawnPosition);
		}

		private void SpawnFloorAt( Vector3 newSpawnPosition )
		{
			var newFloor = Instantiate(_FloorSegment, newSpawnPosition, Quaternion.identity);
			newFloor.Init(_LevelManager, OnFloorDestroyed);
			
			_spawnedFloorSegments.Add(newFloor);
		}

		private void OnFloorDestroyed( LevelSegment segment )
		{
			_spawnedFloorSegments.Remove(segment);
			
			SpawnNewFloorSegmentAtTheEnd();
		}


		#endregion

		#region Obstacles

		private void SpawnObstacles( int amount )
		{
			for ( int i = 0; i < amount; i++ )
			{
				SpawnNewObstacleSegmentAtTheEnd();
			}
		}

		private void SpawnNewObstacleSegmentAtTheEnd()
		{
			var lastObstacle = _spawnedObstacles.Count == 0 ? null : _spawnedObstacles[^1];
			var isFirstObstacle = lastObstacle == null;

			Vector3 obstacleSpawnPosition;
			var randomObstacle = _LevelSegments.RandomElement();
			
			if ( isFirstObstacle )
			{
				var firstFloorSegment = _spawnedFloorSegments[0];
				obstacleSpawnPosition = firstFloorSegment.transform.position + Vector3.forward * firstFloorSegment.GetZLength();
			}
			else
			{
				obstacleSpawnPosition = CalculateNewPositionFromLastObstacle( lastObstacle, randomObstacle );
			}
			
				
			SpawnObstacleAt(randomObstacle, obstacleSpawnPosition);
		}

		private void SpawnObstacleAt( LevelSegment randomObstacle, Vector3 obstacleSpawnPosition )
		{
			var spawnPosition = obstacleSpawnPosition + randomObstacle.SpawnOffset;
			var newObstacle = Instantiate(randomObstacle, spawnPosition, Quaternion.identity);
			newObstacle.Init( _LevelManager, OnObstacleDestroyed );
			
			_spawnedObstacles.Add(newObstacle);
		}

		private void OnObstacleDestroyed( LevelSegment obj )
		{
			_onObstaclePassed?.Invoke();
			_spawnedObstacles.Remove(obj);
			
			SpawnNewObstacleSegmentAtTheEnd();
		}


		private Vector3 CalculateNewPositionFromLastObstacle( LevelSegment lastObstacle, LevelSegment randomObstacle )
		{
			// TODO: Implement this
			return lastObstacle.transform.position + Vector3.forward * 10f - lastObstacle.SpawnOffset;
		}

		#endregion

		public void Init( Action onObstaclePassed )
		{
			_onObstaclePassed = onObstaclePassed;
		}
	}
 
}
