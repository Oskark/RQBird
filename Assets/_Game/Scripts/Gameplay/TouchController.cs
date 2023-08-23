
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

		private float? _beginMovementPosition;
		private int? _beginMovementFingerID;
		public void Evt_Slide( BaseEventData eventData )
		{
			var  noMovementYet = _beginMovementPosition == null;
			if ( noMovementYet )
			{
				RegisterMovement( eventData );
			}
			
			var currentPointerPosition = LastPointerPosition( eventData );
			var delta = currentPointerPosition - _beginMovementPosition;

			var areaToMoveCompletely = _TouchArea.rect.width / 3f; // Approx 1/3 of area to move completely
			delta = delta / areaToMoveCompletely;

			SlidePerformed?.Invoke( delta.Value );
        }

		private void RegisterMovement(BaseEventData eventData)
		{
			var input = eventData.currentInputModule.input;
			if ( Input.touchCount > 0 )
			{
				var oneFingerTouch = Input.touchCount == 1;
				if ( oneFingerTouch )
				{
					var touch = input.GetTouch( 0 );
					_beginMovementPosition = touch.position.x;
					_beginMovementFingerID = touch.fingerId;
				}
                
				var pointerData = eventData as PointerEventData; 
				if ( pointerData != null )
				{
					var touch = input.GetTouch( pointerData.pointerId );
					_beginMovementPosition = touch.position.x;
					_beginMovementFingerID = touch.fingerId;
				}
            }
			
			if (Input.mousePresent)
			{
				_beginMovementPosition = LastPointerPosition( eventData );
				_beginMovementFingerID = 0;
			}
		}

		private float LastPointerPosition( BaseEventData eventData )
		{
			var input = eventData.currentInputModule.input;
			
			var  touchOccured = input.touchCount > 0;
			if ( touchOccured )
			{
				var  oneTouchOccured = input.touchCount <= 1;
				if ( oneTouchOccured ) // One touch - no need to search for the same finger id
				{
					return input.GetTouch( 0 ).position.x;
				}
				
				// More than one touch - find proper touch position
				for ( int i = 0; i < input.touchCount; i++ )
				{
					var touch = input.GetTouch( i );
					var isSameFingerTouch = touch.fingerId == _beginMovementFingerID;
					if ( isSameFingerTouch )
					{
						return touch.position.x;
					}
				}

				// No touch with same finger id found, so we return the first touch position
				return input.GetTouch( 0 ).position.x;
			}
			else // No touches - mouse
			{
				return input.mousePosition.x;
			}
		}

		public void Evt_OnEndSlide( BaseEventData eventData )
		{
			_beginMovementPosition = null;

			SlidePerformed?.Invoke( 0 );
		}
		
	}
}
