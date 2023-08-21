using System;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Gameplay.Levels
{
	public class LevelManager : MonoBehaviour
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

		private void Start()
		{
			_accelerationsCountToIncreaseSpeed = _gameplayData.ObstaclesPassedToAccelerate;
			_basePlayerSpeed = _gameplayData.PlayerMoveSpeed;
			_accelerationPerAdvance = _gameplayData.AccelerationPerAdvance;

			_LevelGenerator.Init(OnObstaclePassed);
		}

		private void OnObstaclePassed()
		{
			_obstaclesPassed++;
			Debug.Log($"Obstacle passed. Current: {_obstaclesPassed}"  );

			if (_obstaclesPassed % _accelerationsCountToIncreaseSpeed == 0)
			{
				_accelerationsAmount++;
				Debug.Log($"Obstacle passed. New acceleration: {_accelerationsAmount}"  );
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
			
			_LevelGenerator.SetPause(isPaused);
		}


		private void UpdateDistancePassed()
		{
			_distancePassed += _basePlayerSpeed * Time.deltaTime;
		}

		public void GenerateLevel()
		{
			_LevelGenerator.GenerateLevel();
		}
	}
}
