
using Gameplay.Levels;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gameplay
{
	[CreateAssetMenu( fileName = "GameplayElementsContainer", menuName = "OneTimeScripts/GameplayElementsContainer" )]
	public class GameplayElementsContainer : ScriptableObject
	{
		[field: SerializeField] public AssetReferenceGameObject FloorSegmentAR { get; private set; }
		[field: SerializeField] public AssetReferenceGameObject[] LevelSegmentsAR { get; private set; }
	}
}
