
using System;
using UnityEngine;

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

		private void Awake()
		{
			TouchController.JumpPressed -= OnJumpPressed;
			TouchController.JumpPressed += OnJumpPressed;
			
			TouchController.SlidePerformed -= OnSlidePerformed;
			TouchController.SlidePerformed += OnSlidePerformed;
		}

		private void Update()
		{
			if (IsPaused) return;

			WasJump = ReadTouchJump() || ReadKeyboardJump();
			SlideChange = ReadTouchSlide() + ReadKeyboardSlide();
        }

		private float ReadTouchSlide()
		{
			return _LastSlideChanged;
		}

		private float ReadKeyboardSlide()
		{
			if (Input.GetKey(KeyCode.A)) return -1;
			if (Input.GetKey(KeyCode.D)) return 1;

			return 0;
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
