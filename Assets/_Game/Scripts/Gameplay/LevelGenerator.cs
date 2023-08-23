using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Gameplay.Levels
{
	public class LevelGenerator : IDisposable
	{
		public const string FLOOR_SPAWNER_ID = "FloorSpawner";
		public const string OBSTACLE_SPAWNER_ID = "ObstacleSpawner";
		
		public event Action OnObstaclePassed;

		[Inject( Id = FLOOR_SPAWNER_ID )] private ILevelSegmentSpawner _floorSpawner;
		[Inject( Id = OBSTACLE_SPAWNER_ID )] private ILevelSegmentSpawner _obstacleSpawner;
		
		private Transform _spawnContainer;
		private SignalBus _signalBus;
		
		[Inject]
		public void Construct( SignalBus signal )
		{
			Debug.Log($"Level Generator Construct"  );

			_signalBus = signal;
			_signalBus.Subscribe<RestartGameSignal>( OnRestart );
			_signalBus.Subscribe<ExitGameplaySignal>( OnRestart );
		}

		private void OnRestart()
		{
			_floorSpawner.CleanUp();
			_obstacleSpawner.CleanUp();
		}

		public void Dispose()
		{
			_signalBus.Unsubscribe<RestartGameSignal>( OnRestart );
			
			if (_spawnContainer != null)
				GameObject.Destroy( _spawnContainer.gameObject );
		}
		
		public LevelSegment GetLastSpawnedFloorSegment()
		{
			return _floorSpawner?.SpawnedSegments.LastElementOrDefault();
		}

		public LevelSegment GetFirstFloorSegmentOrDefault()
		{
			return _floorSpawner?.SpawnedSegments.ElementAtOrDefault( 0 );
		}

		public void GenerateLevel()
		{			
			_spawnContainer = new GameObject("SpawnContainer").transform;
            
			SpawnFloors(  10 );
			SpawnObstacles( 10 );	
		}

        
		private void SpawnFloors( int amount )
		{
			for ( int i = 0; i < amount; i++ )
			{
				SpawnNewSegmentAtTheEndUsing( _floorSpawner );
			}
		}

		private void SpawnObstacles( int amount )
		{
			for ( int i = 0; i < amount; i++ )
			{
				SpawnNewSegmentAtTheEndUsing( _obstacleSpawner );
			}
		}

		private void SpawnNewSegmentAtTheEndUsing(ILevelSegmentSpawner spawner)
		{
			var obstacleSpawnPosition = spawner.GetSpawnPosition();
			
			SpawnSegmentAt( spawner, obstacleSpawnPosition);
		}

		private void SpawnSegmentAt( ILevelSegmentSpawner spawner, Vector3 obstacleSpawnPosition )
		{
			var newObstacle = spawner.GetSegmentInstance();
			var spawnPosition = obstacleSpawnPosition + newObstacle.SpawnOffset;

			newObstacle.transform.position = spawnPosition;
			newObstacle.transform.rotation = Quaternion.identity;
			newObstacle.transform.SetParent( _spawnContainer );
			
			newObstacle.Init( instance =>
			{
				Debug.Log($"{GetType()}: On Element destroyed {instance} {instance.GetInstanceID()}"  );
				if ( spawner.ShouldBeReportedAsPassed )
				{
					OnObstaclePassed?.Invoke();
				}
				
				spawner.OnObstacleDestroyed(instance);
				
				SpawnNewSegmentAtTheEndUsing( spawner );
			} );
        }
        
		public (float left, float right) GetLevelBounds()
		{
			return _floorSpawner.GetSegmentWidth();
		}

	}
 
}
