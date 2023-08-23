using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Gameplay.Levels
{
	public class LevelGenerator : IDisposable
	{
        private Transform _SpawnContainer;
		
		[Inject( Id = "FloorSpawner" )] private ILevelSegmentSpawner _floorSpawner;
		[Inject( Id = "ObstacleSpawner" )] private ILevelSegmentSpawner _obstacleSpawner;

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
			
			if (_SpawnContainer != null)
				GameObject.Destroy( _SpawnContainer.gameObject );
		}
		
		public LevelSegment GetLastSpawnedFloorSegment()
		{
			return _floorSpawner?.SpawnedSegments.LastElementOrDefault();
		}

		public LevelSegment GetFirstFloorSegmentOrDefault()
		{
			return _floorSpawner?.SpawnedSegments.ElementAtOrDefault( 0 );
		}
		
		public event Action OnObstaclePassed;


		public void GenerateLevel()
		{			
			_SpawnContainer = new GameObject("SpawnContainer").transform;
            
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
			newObstacle.transform.SetParent( _SpawnContainer );
			
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
