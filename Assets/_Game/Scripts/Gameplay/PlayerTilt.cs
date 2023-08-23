using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class PlayerTilt : MonoBehaviour
	{
		[SerializeField] private Rigidbody _Rigidbody;

		void Update()
		{
			if (_Rigidbody.velocity.y > 0)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(80, 0, 0), Time.deltaTime * 10f);
			}
			else if (_Rigidbody.velocity.y < 0)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(100, 0, 0), Time.deltaTime * 2f);
			}
			else
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(90, 0, 0), Time.deltaTime * 5f);
			}
		}
	}

}
