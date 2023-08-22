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
		[SerializeField] private PlayerController _PlayerController;

		[Inject] private LevelManager _LevelManager;
		[Inject] private GameplayElementsProvider _GameplayElementsProvider;

		[Inject] private HighScoresManager _HighScoresManager;


		private void Awake()
		{
			Application.targetFrameRate = 60;
			
			_PlayerController.OnPlayerHitSegment -= OnPlayerHitSegment;
			_PlayerController.OnPlayerHitSegment += OnPlayerHitSegment;
        }

		private void Start()
		{
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

		private void OnDestroy()
		{
			if ( _GameplayElementsProvider != null )
			{
				_GameplayElementsProvider.Clear ();
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
		//
		// private void FreezePlayer()
		// {
		// 	_PlayerController.SetPause( true );
		// }
        
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

		private SignalBus _signalBus;
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
