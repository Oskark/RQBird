using UnityEngine;

namespace Gameplay
{
	[CreateAssetMenu( fileName = "GameplayConfig", menuName = "OneTimeScripts/GameplayData" )]
	public class GameplayConfig : ScriptableObject
	{
		[field: Header("PlayerConfig")]
		[field: SerializeField] public float PlayerMoveSpeed { get; private set; } = 1f;
		[field: SerializeField] public float PlayerHorizontalSpeed { get; private set; } = 1f;
		[field: SerializeField] public float PlayerHorizontalSpeed_KeyboardSpeed { get; private set; } = 2f;
		[field: SerializeField] public float PlayerJumpStrength { get; private set; } = 10f;

		[field: Header("Map Settings")]
		[field: SerializeField] public float CeilingHeight { get; private set; } = 10f;

		[field: Header( "Level Settings" )]
		[field: SerializeField]
		public float FirstObstacleSpawnDistanceFromFirstFloorEnd { get; set; } = 10f;
		[field: SerializeField] public int ObstaclesPassedToAccelerate { get; private set; } = 3;
		[field: SerializeField] public float AccelerationPerAdvance { get; private set; } = 0.1f;

		[field: SerializeField] public float ObstaclesDistance { get; private set; } = 10f;


		[field: Header( "Pool settings" )] 
		[field: SerializeField] public int EntryPoolDefaultCapacity  { get; private set; }= 3;
		[field: SerializeField] public int EntryPoolMaxCapacity { get; private set; }= 5;
		
		[field: Header( "High scores settings" )]
		[field: SerializeField] public int SavedHighScoresAmount { get; private set; }= 20;

	}
}
