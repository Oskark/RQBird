using System.Collections.Generic;
using Gameplay.Levels;
using UnityEngine;

public interface ILevelSegmentSpawner
{
	bool ShouldBeReportedAsPassed { get; }
	
	List<LevelSegment> SpawnedSegments { get; }

	(float, float) GetSegmentWidth();
	
	
	Vector3 GetSpawnPosition();

	LevelSegment GetSegmentInstance();
	
	void OnObstacleDestroyed(LevelSegment instance);

	void CleanUp();
}
