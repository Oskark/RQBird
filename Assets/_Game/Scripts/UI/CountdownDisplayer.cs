
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Gameplay.UI
{
	public class CountdownDisplayer : MonoBehaviour
	{

		[Header( "Config" )] [SerializeField] private bool _IsEnabled = true;
		[SerializeField] private int _CountFrom = 3;

		[Header( "Refs" )] [SerializeField] private TextMeshProUGUI _CountdownValue;

		public void ShowCountdown( Action onCountdownFinished )
		{
			if ( !_IsEnabled )
			{
				onCountdownFinished?.Invoke();
				return;
			}

			ShowCountdown( _CountFrom, onCountdownFinished );
		}

		private void ShowCountdown( int countFrom, Action onFinish )
		{
			StartCoroutine( CountdownRoutine( countFrom, onFinish ) );
		}

		private IEnumerator CountdownRoutine( int countFrom, Action onFinish )
		{
			for ( int i = countFrom; i > 0; i-- )
			{
				_CountdownValue.text = i.ToString();

				yield return new WaitForSeconds( 0.75f );
			}

			_CountdownValue.text = "GO!";

			yield return new WaitForSeconds( 0.75f );

			onFinish?.Invoke();
		}

	}
}
