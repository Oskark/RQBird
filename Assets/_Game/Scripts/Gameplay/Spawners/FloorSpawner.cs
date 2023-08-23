using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using Gameplay.Levels;
using Zenject;

public class FloorSpawner : ILevelSegmentSpawner
{

	[Inject] private LevelGenerator _levelGenerator;
	[Inject] private GameplayElementsProvider _gameplayElementsProvider;
    
	public bool ShouldBeReportedAsPassed => false;
	
	public List<LevelSegment> SpawnedSegments { get; protected set; } = new List<LevelSegment>();

	public (float, float) GetSegmentWidth()
	{
		if (SpawnedSegments.Count > 0 && SpawnedSegments[0] != null)
		{
			return SpawnedSegments[0].GetLeftRightBounds();
		}

		return _gameplayElementsProvider.GetFloorLeftRightSegment();

	}

	public Vector3 GetSpawnPosition()
	{
		var lastFloor = _levelGenerator.GetLastSpawnedFloorSegment();

		var isFirstSegment = lastFloor == null;
		var firstElementPosition = Vector3.zero;
			
		var newSpawnPosition = isFirstSegment ? firstElementPosition : lastFloor.transform.position + Vector3.forward * lastFloor.GetZLength();

		return newSpawnPosition;
	}

	public LevelSegment GetSegmentInstance()
	{
		var instance =  _gameplayElementsProvider.GetFloor();
		
		SpawnedSegments.Add( instance );

		return instance;
	}

	public void OnObstacleDestroyed( LevelSegment instance )
	{
		_gameplayElementsProvider.ReturnSegment( instance );
		
		SpawnedSegments.Remove( instance );
	}
}
