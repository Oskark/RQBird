using Gameplay.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.Levels
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField] private LevelManager _LevelManager;
		[SerializeField] private PlayerController _PlayerController;
		[SerializeField] private HighScoresManager _HighScoresManager;

		[SerializeField] private GameplayHUD _GameplayHUD;

		private void Awake()
		{
			_PlayerController.OnPlayerHitSegment -= OnPlayerHitSegment;
			_PlayerController.OnPlayerHitSegment += OnPlayerHitSegment;
		}

		private void OnPlayerHitSegment()
		{
			_PlayerController.OnPlayerHitSegment -= OnPlayerHitSegment;
			
			_LevelManager.SetPause(true);
			
			var obtainedScore = (int) _LevelManager.DistancePassed;
			_HighScoresManager.Init( () =>
			{
				_HighScoresManager.RegisterScore( obtainedScore, out var highScorePosition, out var lowestHighScoreValue );
			
				_GameplayHUD.ShowGameOverPanel( obtainedScore, highScorePosition, lowestHighScoreValue, OnRestart, OnExit );

			} );
		}

		private void OnRestart()
		{
			SceneManager.UnloadSceneAsync( "Gameplay" );
			SceneManager.LoadScene( "Gameplay" );
		}

		private void OnExit()
		{
			SceneManager.UnloadSceneAsync( "Gameplay" );
			SceneManager.LoadScene( "MainMenu" );
		}
	}
}
