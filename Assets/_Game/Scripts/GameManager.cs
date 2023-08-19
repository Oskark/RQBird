using Gameplay.UI;
using UnityEngine;

namespace Gameplay.Levels
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField] private LevelManager _LevelManager;
		[SerializeField] private PlayerController _PlayerController;

		[SerializeField] private GameplayHUD _GameplayHUD;

		private void Awake()
		{
			_PlayerController.OnPlayerHitSegment -= OnPlayerHitSegment;
			_PlayerController.OnPlayerHitSegment += OnPlayerHitSegment;
		}

		private void OnPlayerHitSegment()
		{
			_LevelManager.SetPause(true);
			
			_GameplayHUD.ShowGameOverPanel( (int) _LevelManager.DistancePassed, 1, null );
		}
	}
}
