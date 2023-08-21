using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Gameplay.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Gameplay.Levels
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField] private LevelManager _LevelManager;
		[SerializeField] private PlayerController _PlayerController;
		[SerializeField] private HighScoresManager _HighScoresManager;

		[SerializeField] private GameplayHUD _GameplayHUD;

		[Inject] private GameplayElementsProvider _GameplayElementsProvider;

		private void Awake()
		{
			Application.targetFrameRate = 60;
			
			_PlayerController.OnPlayerHitSegment -= OnPlayerHitSegment;
			_PlayerController.OnPlayerHitSegment += OnPlayerHitSegment;

			PrepareGame();
		}

		private void OnDestroy()
		{
			if ( _GameplayElementsProvider != null )
			{
				_GameplayElementsProvider.Clear ();
			}
		}

		private async void PrepareGame()
		{
			FreezePlayer();
			FreezeLevel();

			await PreloadMapIfNeeded();

			GenerateLevel();
			
			StartCountdown();
		}

		[Inject] private GameInstaller _gameInstaller;
		private async Task PreloadMapIfNeeded()
		{
			await _GameplayElementsProvider.PreloadElements( 
					spawnFunc:	element => _gameInstaller.SpawnInjectableObject( element ).GetComponent<LevelSegment>() );
		}

		private void GenerateLevel()
		{
			_LevelManager.GenerateLevel();
		}

		private void FreezePlayer()
		{
			_PlayerController.SetPause( true );
		}

		private void FreezeLevel()
		{
			_LevelManager.SetPause( true );
		}

		private void StartCountdown()
		{
			_GameplayHUD.ShowCountdown( OnCountdownFinished );
		}

		private void OnCountdownFinished()
		{
			_PlayerController.SetPause( false );
			_LevelManager.SetPause( false );
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
			SceneManager.LoadScene( "Gameplay" );
		}

		private void OnExit()
		{
			SceneManager.LoadScene( "MainMenu" );
		}
	}
}
