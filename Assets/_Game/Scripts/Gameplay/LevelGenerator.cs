using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Gameplay.Levels
{
	public class LevelGenerator : MonoBehaviour
	{
        
		[Inject] private GameplayElementsProvider _gameplayElementsProvider;
		

		[SerializeField] private Transform _SpawnContainer;
        
        
		private List<LevelSegment> _spawnedFloorSegments = new List<LevelSegment>();
		private List<LevelSegment> _spawnedObstacles = new List<LevelSegment>();

		public event Action OnObstaclePassed;


		public void GenerateLevel()
		{
			SpawnFloors(  10 );
			SpawnObstacles( 10 );	
		}
		

		#region Floors

		private void SpawnFloors( int amount )
		{
			for ( int i = 0; i < amount; i++ )
			{
				Debug.Log($"Spawning floor {i}"  );
				SpawnNewFloorSegmentAtTheEnd();
			}
		}

		private void SpawnNewFloorSegmentAtTheEnd()
		{
			var lastFloor = _spawnedFloorSegments.Count == 0 ? null : _spawnedFloorSegments[^1];

			var isFirstSegment = lastFloor == null;
			var newSpawnPosition = isFirstSegment ? Vector3.zero : lastFloor.transform.position + Vector3.forward * lastFloor.GetZLength();
				
			SpawnFloorAt(newSpawnPosition);
		}

		private void SpawnFloorAt( Vector3 newSpawnPosition )
		{
			var newFloor = _gameplayElementsProvider.GetFloor();
			newFloor.transform.position = newSpawnPosition;
			newFloor.transform.rotation = Quaternion.identity;
			newFloor.transform.SetParent( _SpawnContainer );
			
			newFloor.Init(OnFloorDestroyed);
            _spawnedFloorSegments.Add(newFloor);
		}

		private void OnFloorDestroyed( LevelSegment segment )
		{
			_gameplayElementsProvider.ReturnSegment( segment );
			
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
			
			if ( isFirstObstacle )
			{
				var firstFloorSegment = _spawnedFloorSegments[0];
				obstacleSpawnPosition = firstFloorSegment.transform.position + Vector3.forward * (firstFloorSegment.GetZLength() * 2);
			}
			else
			{
				obstacleSpawnPosition = CalculateNewPositionFromLastObstacle( lastObstacle );
			}
			
				
			SpawnObstacleAt( obstacleSpawnPosition);
		}

		private void SpawnObstacleAt( Vector3 obstacleSpawnPosition )
		{
			var newObstacle = _gameplayElementsProvider.GetRandomSegment();
			
			var spawnPosition = obstacleSpawnPosition + newObstacle.SpawnOffset;

			newObstacle.transform.position = spawnPosition;
			newObstacle.transform.rotation = Quaternion.identity;
			newObstacle.transform.SetParent( _SpawnContainer );
			
			newObstacle.Init( OnObstacleDestroyed );
            
			_spawnedObstacles.Add(newObstacle);
		}

		private void OnObstacleDestroyed( LevelSegment segment )
		{
			OnObstaclePassed?.Invoke();
			
			_gameplayElementsProvider.ReturnSegment( segment );

			_spawnedObstacles.Remove(segment);
			
			SpawnNewObstacleSegmentAtTheEnd();
		}


		private Vector3 CalculateNewPositionFromLastObstacle( LevelSegment lastObstacle )
		{
			// TODO: Implement this
			return lastObstacle.transform.position + Vector3.forward * 10f - lastObstacle.SpawnOffset;
		}

		#endregion

		public (float left, float right) GetLevelBounds()
		{
			if (_spawnedFloorSegments.Count > 0 && _spawnedFloorSegments[0] != null)
			{
				return _spawnedFloorSegments[0].GetLeftRightBounds();
			}

			return _gameplayElementsProvider.GetFloorLeftRightSegment();
		}

	}
 
}
