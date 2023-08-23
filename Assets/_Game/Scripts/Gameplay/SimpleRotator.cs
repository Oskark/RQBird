#region genericScriptable
// Created by Oskar Kucharczyk at 23:35
#endregion

using System;
using UnityEngine;

namespace Gameplay.Levels
{
	public class SimpleRotator : MonoBehaviour
	{
		[SerializeField] private Vector2 _RotationSpeedRange; 
		// [SerializeField] private float _rotationSpeed = 10f;
		[SerializeField] private Vector3 _RotationAxis = Vector3.up;

		private float _rotationSpeed;
		
		private void OnEnable()
		{
			transform.rotation = Quaternion.identity;
			
			_rotationSpeed = UnityEngine.Random.Range( _RotationSpeedRange.x, _RotationSpeedRange.y );
		}

		private void Update()
		{
			transform.Rotate(_RotationAxis, _rotationSpeed * Time.deltaTime);
		}
	}
}
