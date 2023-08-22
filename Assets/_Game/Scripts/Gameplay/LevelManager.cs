using System;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Gameplay.Levels
{
	public class LevelManager : MonoBehaviour, IInitializable
	{
		[Inject] private GameplayData _gameplayData;
		[Inject] private LevelGenerator _LevelGenerator;

		public float DistancePassed => _distancePassed;
		public float CurrentSpeed => _basePlayerSpeed + _accelerationPerAdvance * _accelerationsAmount;
		
		private int _obstaclesPassed = 0;
		private int _accelerationsAmount = 0;
		
		private float _distancePassed = 0;
		private bool _isPaused = false;
		
		private int _accelerationsCountToIncreaseSpeed;
		private float _basePlayerSpeed;
		private float _accelerationPerAdvance;

		private SignalBus _signalBus;

		private void Start()
		{
			_accelerationsCountToIncreaseSpeed = _gameplayData.ObstaclesPassedToAccelerate;
			_basePlayerSpeed = _gameplayData.PlayerMoveSpeed;
			_accelerationPerAdvance = _gameplayData.AccelerationPerAdvance;

			_LevelGenerator.OnObstaclePassed -= OnObstaclePassed;
			_LevelGenerator.OnObstaclePassed += OnObstaclePassed;
		}
        
		[Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;
			_signalBus.Subscribe<GameplayStateChangedSignal>( OnGameStateChanged );
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

		private void Update()
		{
			if (_isPaused) return;

			UpdateDistancePassed();
		}
		
		public (float left, float right) GetLevelBounds()
		{
			return _LevelGenerator.GetLevelBounds();
		}
		
		public void SetPause(bool isPaused)
		{
			_isPaused = isPaused;
        }


		private void UpdateDistancePassed()
		{
			_distancePassed += _basePlayerSpeed * Time.deltaTime;
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
