using UnityEngine;

namespace Gameplay
{
	[CreateAssetMenu( fileName = "GameplayData", menuName = "OneTimeScripts/GameplayData" )]
	public class GameplayData : ScriptableObject
	{
		[field: Header("PlayerConfig")]
		[field: SerializeField] public float PlayerMoveSpeed { get; private set; } = 1f;
		[field: SerializeField] public float PlayerHorizontalSpeed { get; private set; } = 1f;
		[field: SerializeField] public float PlayerJumpStrength { get; private set; } = 10f;

		[field: Header("Map Settings")]
		[field: SerializeField] public float CeilingHeight { get; private set; } = 10f;

		[field: Header("Level Settings")]
		[field: SerializeField] public int ObstaclesPassedToAccelerate { get; private set; } = 3;
		[field: SerializeField] public float AccelerationPerAdvance { get; private set; } = 0.1f;


		[field: Header( "Pool settings" )] 
		[field: SerializeField] public int EntryPoolDefaultCapacity = 3;
		[field: SerializeField] public int EntryPoolMaxCapacity = 5;
		
	}
}
