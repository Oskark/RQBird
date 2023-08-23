using UnityEngine;
using Zenject;

namespace Gameplay.Levels
{
	public class LevelManager : ITickable, IInitializable
	{
		[Inject] private GameplayData _gameplayData;
		[Inject] private LevelGenerator _LevelGenerator;

		public float TimeSurvived => _timeSurvived;
		public float CurrentSpeed => _basePlayerSpeed + _accelerationPerAdvance * _accelerationsAmount;
		
		private int _obstaclesPassed = 0;
		private int _accelerationsAmount = 0;
		
		private float _timeSurvived = 0;
		private bool _isPaused = false;
		
		private int _accelerationsCountToIncreaseSpeed;
		private float _basePlayerSpeed;
		private float _accelerationPerAdvance;

		private SignalBus _signalBus;

        
		[Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;
			_signalBus.Subscribe<GameplayStateChangedSignal>( OnGameStateChanged );
			_signalBus.Subscribe<RestartGameSignal>( ResetData );
			_signalBus.Subscribe<ExitGameplaySignal>( ResetData );
			
			ResetData();
		}

		private void ResetData()
		{
			_accelerationsCountToIncreaseSpeed = _gameplayData.ObstaclesPassedToAccelerate;
			_basePlayerSpeed = _gameplayData.PlayerMoveSpeed;
			_accelerationPerAdvance = _gameplayData.AccelerationPerAdvance;

			_timeSurvived = 0;
			_isPaused = false;

			_obstaclesPassed = 0;
			_accelerationsAmount = 0;
			
			_LevelGenerator.OnObstaclePassed -= OnObstaclePassed;
			_LevelGenerator.OnObstaclePassed += OnObstaclePassed;
		}

		private void OnGameStateChanged( GameplayStateChangedSignal newState )
		{
			var isPlaying = newState.CurrentState == GameState.Play;
			
			SetPause( isPlaying == false );
		}



		private void OnObstaclePassed()
		{
			_obstaclesPassed++;
            
			if (_obstaclesPassed % _accelerationsCountToIncreaseSpeed == 0)
			{
				_accelerationsAmount++;
			}
		}

		public void Tick()
		{
			if (_isPaused) return;

			UpdateTime();
		}
		
		public (float left, float right) GetLevelBounds()
		{
			return _LevelGenerator.GetLevelBounds();
		}
		
		public void SetPause(bool isPaused)
		{
			_isPaused = isPaused;
        }


		private void UpdateTime()
		{
			_timeSurvived += Time.deltaTime;
		}

		public void GenerateLevel()
		{
			_LevelGenerator.GenerateLevel();
		}

		public void Initialize()
		{
		}


	}
}
