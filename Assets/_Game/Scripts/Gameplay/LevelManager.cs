using System;
using UnityEngine;

namespace Gameplay.Levels
{
	public interface ILevelManagerable
	{
		public float SpeedModifier { get; }
		
		public (float left, float right) GetLevelBounds(); 
	}
	public class LevelManager : MonoBehaviour, ILevelManagerable
	{
		[Header("Neccessary refs")]
		[SerializeField] private LevelGenerator _LevelGenerator;

		
		[Header("Settings")]
		[SerializeField] private float _SpeedPerSec = 1f;
		[SerializeField] private float _AccelerationPerSec = 0.1f;

		private float _distancePassed = 0;
		private bool _isPaused = false;

		public float DistancePassed => _distancePassed;
		public float SpeedModifier => _SpeedPerSec + _AccelerationPerSec * _currentAccelerations;
		
		private int _accelerationsCount = 0;
		private readonly int _accelerationsCountToIncreaseSpeed = 3;
		private int _currentAccelerations = 0;

		private void Start()
		{
			_LevelGenerator.Init(OnObstaclePassed);
		}

		private void OnObstaclePassed()
		{
			_accelerationsCount++;
			if (_accelerationsCount % _accelerationsCountToIncreaseSpeed == 0)
			{
				_currentAccelerations++;
			}
		}

		private void Update()
		{
			if (_isPaused) return;

			UpdateDistancePassed();
			ApplyCurrentDistanceToLevelGenerator();
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



		private void ApplyCurrentDistanceToLevelGenerator()
		{
			_LevelGenerator.UpdateCurrentDistance(_distancePassed);
		}

		private void UpdateDistancePassed()
		{
			_distancePassed += _SpeedPerSec * Time.deltaTime;
		}
	}
}
