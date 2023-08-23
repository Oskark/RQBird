
	using System.Collections.Generic;
	using Gameplay;
	using Gameplay.Levels;
	using UnityEngine;
	using Zenject;

	public class ObstacleSpawner : ILevelSegmentSpawner
	{
		[Inject] private LevelGenerator _levelGenerator;
		[Inject] private GameplayData _gameplayData;
		[Inject] private GameplayElementsProvider _gameplayElementsProvider;
        
		public bool ShouldBeReportedAsPassed => true;

		public List<LevelSegment> SpawnedSegments { get; } = new List<LevelSegment>();

		public (float, float) GetSegmentWidth()
		{
			// Obstacle width doesn't matter
			return (-1, -1);
		}

		public Vector3 GetSpawnPosition()
		{
			Vector3 obstacleSpawnPosition;
			
			var lastObstacle = SpawnedSegments.LastElementOrDefault();

			var  isFirstEverObstacle = lastObstacle == null;
			if ( isFirstEverObstacle )
			{
				obstacleSpawnPosition = CalculatePositionIfFirstElement();
			}
			else 
			{
				obstacleSpawnPosition = CalculateNewPositionFromLastObstacle( lastObstacle );
			}

			return obstacleSpawnPosition;
		}
		
		public LevelSegment GetSegmentInstance()
		{
			var instance = _gameplayElementsProvider.GetRandomSegment();

			SpawnedSegments.Add( instance );

			return instance;
		}

		public void OnObstacleDestroyed( LevelSegment instance )
		{
			SpawnedSegments.Remove( instance );
			_gameplayElementsProvider.ReturnSegment( instance );
		}

		private Vector3 CalculatePositionIfFirstElement(  )
		{
			const int FALLBACK_LENGTH = 10;

			var firstFloorSegment = _levelGenerator.GetFirstFloorSegmentOrDefault();
			var firstFloorLength = firstFloorSegment != null ? firstFloorSegment.GetZLength() : FALLBACK_LENGTH;
			var firstFloorPosition = firstFloorSegment != null ? firstFloorSegment.transform.position : Vector3.zero;

			var firstObstacleSpawnDistance = _gameplayData.FirstObstacleSpawnDistanceFromFirstFloorEnd;
			return firstFloorPosition + Vector3.forward * (firstFloorLength + firstObstacleSpawnDistance);
		}
        
		private Vector3 CalculateNewPositionFromLastObstacle( LevelSegment lastObstacle )
		{
			var lastObstaclePosition = lastObstacle.transform.position;
			var distanceBetweenObstacles = _gameplayData.ObstaclesDistance * Vector3.forward;
			var obstacleSpecificOffset = lastObstacle.SpawnOffset;
			
			return lastObstaclePosition + distanceBetweenObstacles - obstacleSpecificOffset;
		}

		public void CleanUp()
		{
			SpawnedSegments.ForEach( segment => _gameplayElementsProvider.ReturnSegment( segment ) );
			
			SpawnedSegments.Clear();
		}
	}
