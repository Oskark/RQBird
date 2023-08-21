#region genericScriptable
// Created by Oskar Kucharczyk at 20:58
#endregion

using UnityEngine;

namespace Gameplay
{
	[CreateAssetMenu( fileName = "GameplayData", menuName = "OneTimeScripts/GameplayData" )]
	public class GameplayData : ScriptableObject
	{
		[field: SerializeField] public float PlayerJumpStrength { get; private set; } = 10f;
		[field: SerializeField] public float PlayerHorizontalSpeed { get; private set; } = 1f;
		
		[field: SerializeField] public float CeilingHeight { get; private set; } = 10f;
		
	}
}
