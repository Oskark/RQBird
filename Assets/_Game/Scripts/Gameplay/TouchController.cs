
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay
{
	public class TouchController : MonoBehaviour
	{

		public static Action JumpPressed;
		public static Action<float> SlidePerformed;

		[SerializeField] private RectTransform _TouchArea;
		
		public void Evt_Jump()
		{
			JumpPressed?.Invoke();
		}

		private float? _LastPointerPosition;

		public void Evt_OnBeginSlide( BaseEventData eventData )
		{
			_LastPointerPosition = eventData.currentInputModule.input.GetTouch( 0 ).position.x;
		}
		public void Evt_Slide( BaseEventData eventData )
		{
			_LastPointerPosition ??= LastPointerPosition( eventData );
			
			var currentPointerPosition = LastPointerPosition( eventData );
			var delta = currentPointerPosition - _LastPointerPosition.Value;
			
			Debug.Log($"Delta: {delta}"  );
			delta /= _TouchArea.sizeDelta.x;
			
			SlidePerformed?.Invoke( delta );

			_LastPointerPosition = currentPointerPosition;
		}

		private static float LastPointerPosition( BaseEventData eventData )
		{
			var input = eventData.currentInputModule.input;
			if ( input.touchCount > 0 )
				return input.GetTouch( 0 ).position.x;
			else
				return input.mousePosition.x;
		}

		public void Evt_OnEndSlide( BaseEventData eventData )
		{
			_LastPointerPosition = null;
			Debug.Log($"Delta = 0"  );
			
			SlidePerformed?.Invoke( 0 );
		}
		
	}
}
