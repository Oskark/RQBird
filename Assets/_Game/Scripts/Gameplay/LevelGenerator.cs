using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Gameplay.Levels
{
	public class LevelGenerator : MonoBehaviour
	{
		
		[SerializeField] private bool _isPaused = false;
		
		[Inject] private GameplayElementsProvider _gameplayElementsProvider;
		
		[SerializeField] private LevelSegment[] _LevelSegments;
		[SerializeField] private LevelSegment _FloorSegment;

		[SerializeField] private Transform _SpawnContainer;
        
		
		[Inject] private GameInstaller _gameInstaller;
		
		private List<LevelSegment> _spawnedFloorSegments = new List<LevelSegment>();
		private List<LevelSegment> _spawnedObstacles = new List<LevelSegment>();

		private Action _onObstaclePassed;

		
		
		public void GenerateLevel()
		{
			SpawnFloors(  10 );
			SpawnObstacles( 10 );	
		}

		// private void Start()
		// {
		// 	SpawnFloors(  10 );
		// 	SpawnObstacles( 10 );
		// }

		void Update()
		{
			if (_isPaused) return;
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

			var isFirstSegment = lastFloor == null;
			var newSpawnPosition = isFirstSegment ? Vector3.zero : lastFloor.transform.position + Vector3.forward * lastFloor.GetZLength();
				
			SpawnFloorAt(newSpawnPosition);
		}

		private void SpawnFloorAt( Vector3 newSpawnPosition )
		{
			// var newFloor = Instantiate(_FloorSegment, newSpawnPosition, Quaternion.identity, _SpawnContainer);
			// var newFloor = _gameInstaller.SpawnInjectableObject( _FloorSegment.gameObject ).GetComponent<LevelSegment>();
			var newFloor = _gameplayElementsProvider.GetFloor();
			newFloor.transform.position = newSpawnPosition;
			newFloor.transform.rotation = Quaternion.identity;
			newFloor.transform.SetParent( _SpawnContainer );
			
			newFloor.Init(OnFloorDestroyed);
			newFloor.SetPause( _isPaused );
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
			// var newObstacle = Instantiate(randomObstacle, spawnPosition, Quaternion.identity, _SpawnContainer);
			// var newObstacle = _gameInstaller.SpawnInjectableObject( randomObstacle.gameObject ).GetComponent<LevelSegment>();
			newObstacle.transform.position = spawnPosition;
			newObstacle.transform.rotation = Quaternion.identity;
			newObstacle.transform.SetParent( _SpawnContainer );
			
			newObstacle.Init( OnObstacleDestroyed );
			newObstacle.SetPause( _isPaused );
			
			_spawnedObstacles.Add(newObstacle);
		}

		private void OnObstacleDestroyed( LevelSegment segment )
		{
			_onObstaclePassed?.Invoke();
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

		public void Init( Action onObstaclePassed )
		{
			_onObstaclePassed = onObstaclePassed;
		}

		public void SetPause( bool isPaused )
		{
			_isPaused = isPaused;
			
			_spawnedFloorSegments.ForEach( arg => arg.SetPause(isPaused) );
			_spawnedObstacles.ForEach( arg => arg.SetPause(isPaused) );
		}

		public (float left, float right) GetLevelBounds()
		{
			if (_spawnedFloorSegments.Count > 0 && _spawnedFloorSegments[0] != null)
			{
				return _spawnedFloorSegments[0].GetLeftRightBounds();
			}
			
			return _FloorSegment.GetLeftRightBounds();
		}

	}
 
}
