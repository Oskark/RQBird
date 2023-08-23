using System;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Gameplay
{
	[UsedImplicitly]
	public class InputManager : ITickable, IDisposable
	{
		[Inject] private GameplayData _gameplayData;
		
		public bool WasJump { get; private set; }
		public float SlideChange { get; private set; }

		private bool IsPaused { get; set; }

		private bool _JumpFromTouchWasPressed = false;
		private float _LastSlideChanged = 0;

		private SignalBus _signalBus;
        

		public void Dispose()
		{
			Debug.Log($"Dispose"  );
			_signalBus.Unsubscribe<GameplayStateChangedSignal>( OnGameStateChanged );
			
			TouchController.JumpPressed -= OnJumpPressed;
			TouchController.SlidePerformed -= OnSlidePerformed;
		}
        
		[Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;
			_signalBus.Subscribe<GameplayStateChangedSignal>( OnGameStateChanged );
			
			TouchController.JumpPressed -= OnJumpPressed;
			TouchController.JumpPressed += OnJumpPressed;
			
			TouchController.SlidePerformed -= OnSlidePerformed;
			TouchController.SlidePerformed += OnSlidePerformed;
		}
		

		private void OnGameStateChanged( GameplayStateChangedSignal newGameState )
		{
			var  isPlaying = newGameState.CurrentState == GameState.Play;

			IsPaused = !isPlaying;
		}
		
		public void Tick()
		{
			if ( IsPaused ) return;

			WasJump = ReadTouchJump() || ReadKeyboardJump();
			SlideChange = ReadTouchSlide() + ReadKeyboardSlide();

		}
        
		private float ReadTouchSlide()
		{
			return _LastSlideChanged;
		}

		private float _currentKeyboardSlide = 0;
		private float ReadKeyboardSlide()
		{
			if  (Input.GetKey(KeyCode.A) )
			{
				var  wasMovingRight = _currentKeyboardSlide > 0;
				if ( wasMovingRight ) _currentKeyboardSlide = 0;
				
				_currentKeyboardSlide -= Time.deltaTime * _gameplayData.PlayerHorizontalSpeed_KeyboardSpeed;
			}
			else if ( Input.GetKey( KeyCode.D ) )
			{
				var wasMovingLeft = _currentKeyboardSlide < 0;
				if (wasMovingLeft) _currentKeyboardSlide = 0;
			
				_currentKeyboardSlide += Time.deltaTime * _gameplayData.PlayerHorizontalSpeed_KeyboardSpeed;
			}
			else
			{
				_currentKeyboardSlide = 0;
			}

			return _currentKeyboardSlide;
		}


		private bool ReadTouchJump()
		{
			if (!_JumpFromTouchWasPressed) return false;

			_JumpFromTouchWasPressed = false;
			return true;
		}
		
		private static bool ReadKeyboardJump()
		{
			return Input.GetKeyDown(KeyCode.Space);
		}
		
		private void OnJumpPressed()
		{
			_JumpFromTouchWasPressed = true;
		}


		private void OnSlidePerformed(float change)
		{
			_LastSlideChanged = change;
		}


	}
}
