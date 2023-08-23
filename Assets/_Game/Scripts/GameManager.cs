using System;
using System.Threading.Tasks;
using Gameplay.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Gameplay.Levels
{
	public class GameManager : MonoBehaviour, IInitializable, IDisposable
	{
		[SerializeField] private GameplayHUD _GameplayHUD;
		
		[Inject] private PlayerController _PlayerController;
		[Inject] private LevelManager _LevelManager;
		[Inject] private GameplayElementsProvider _GameplayElementsProvider;

		[Inject] private IHighScorable _HighScoresManager;

		private SignalBus _signalBus;


		private void Awake()
		{
			Application.targetFrameRate = 60;
			
        }

		private void Start()
		{
			_PlayerController.OnPlayerHitSegment -= OnPlayerHitSegment;
			_PlayerController.OnPlayerHitSegment += OnPlayerHitSegment;

			PrepareGame();
		}

		private void Update()
		{
			if ( Input.GetKeyDown( KeyCode.E ) )
			{
				ChangeStateTo( GameState.Pause );
			}
			
			if ( Input.GetKeyDown( KeyCode.Q ) )
			{
				ChangeStateTo( GameState.Play );
			}
		}

		private void ChangeStateTo(GameState state)
		{
			Debug.Log($"Changed state to: {state}"  );
			// OnGameStateChanged?.Invoke( state );

			_signalBus.Fire( new GameplayStateChangedSignal { CurrentState = state } );
		}

		private async void PrepareGame()
		{
			ChangeStateTo( GameState.Loading );
            
			// FreezePlayer();
			// FreezeLevel();

			await PreloadMapIfNeeded();

			GenerateLevel();
			
			ChangeStateTo( GameState.Countdown );
			StartCountdown();
        }

		private async Task PreloadMapIfNeeded()
		{
			await _GameplayElementsProvider.PreloadElements();
		}

		private void GenerateLevel()
		{
			_LevelManager.GenerateLevel();
		}

		private void StartCountdown()
		{
			_GameplayHUD.ShowCountdown( OnCountdownFinished );
		}

		private void OnCountdownFinished()
		{
			_PlayerController.SetPause( false );
			_LevelManager.SetPause( false );
			
			ChangeStateTo( GameState.Play );
		}

		private void OnPlayerHitSegment()
		{
			ChangeStateTo( GameState.Result );

			_PlayerController.OnPlayerHitSegment -= OnPlayerHitSegment;
            
			var obtainedScore = (int) _LevelManager.DistancePassed;
			
			_HighScoresManager.RegisterScoreAndThen( obtainedScore, OnScoreRegistered );
			
			void OnScoreRegistered(int highScorePosition, int lowestHighScoreValue)
			{
				_GameplayHUD.ShowGameOverPanel( obtainedScore, highScorePosition, lowestHighScoreValue, OnRestart, OnExit );
			}
        }

		private void OnRestart()
		{
			_signalBus.Fire( new RestartGameSignal()  );
			
			SceneManager.LoadScene( "Gameplay" );
		}

		private void OnExit()
		{
			_signalBus.Fire( new ExitGameplaySignal() );
			
			SceneManager.LoadScene( "MainMenu" );
		}

		public void Initialize()
		{
			
		}
		
		[Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;
			Debug.Log($"Construct with signalBus: {_signalBus}"  );
		}

		public void Dispose()
		{
			
		}
	}
}
