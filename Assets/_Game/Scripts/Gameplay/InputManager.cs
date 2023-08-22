
using System;
using Gameplay.Levels;
using UnityEngine;
using Zenject;

namespace Gameplay
{
	
	public interface IInputManagerable
	{
		public bool WasJump { get; }
		public float SlideChange { get; }
	}
	
	public class InputManager : MonoBehaviour, IInputManagerable
	{
		
		public bool WasJump { get; private set; }
		public float SlideChange { get; private set; }
        
		public bool IsPaused { get; set; }

		private bool _JumpFromTouchWasPressed = false;
		private float _LastSlideChanged = 0;

		private SignalBus _signalBus;
		
		
		[Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;
			_signalBus.Subscribe<GameplayStateChangedSignal>( OnGameStateChanged );
		}


		protected void Awake()
		{
			TouchController.JumpPressed -= OnJumpPressed;
			TouchController.JumpPressed += OnJumpPressed;
			
			TouchController.SlidePerformed -= OnSlidePerformed;
			TouchController.SlidePerformed += OnSlidePerformed;
		}

		private void OnDestroy()
		{
			_signalBus.Unsubscribe<GameplayStateChangedSignal>( OnGameStateChanged );
		}

		private void OnGameStateChanged( GameplayStateChangedSignal newGameState )
		{
			var  isPlaying = newGameState.CurrentState == GameState.Play;

			IsPaused = !isPlaying;
		}

		private void Update()
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
				
				_currentKeyboardSlide -= Time.deltaTime * 2f;
			}
			else if ( Input.GetKey( KeyCode.D ) )
			{
				var wasMovingLeft = _currentKeyboardSlide < 0;
				if (wasMovingLeft) _currentKeyboardSlide = 0;
			
				_currentKeyboardSlide += Time.deltaTime * 2f;
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
