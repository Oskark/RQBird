
using System;
using UnityEngine;

namespace Gameplay.UI
{
	public class GameplayHUD : MonoBehaviour
	{
		[SerializeField] private CountdownDisplayer _CountdownDisplayer;
		[SerializeField] private GameOverPanel _GameOverPanel;

		public void ShowCountdown(Action onFinish)
		{
			_CountdownDisplayer.gameObject.SetActive( true );
			
			_CountdownDisplayer.ShowCountdown( OnCountdownFinished );

			void OnCountdownFinished()
			{
				_CountdownDisplayer.gameObject.SetActive( false );
				
				onFinish?.Invoke();
			}
		}
		
		public void ShowGameOverPanel( int score, int highScorePosition, int? lowestHighScoreValue, Action onRestart, Action onExit )
		{
			_GameOverPanel.gameObject.SetActive( true );
			
			if ( highScorePosition >= 0 )
			{
				_GameOverPanel.ShowWithHighscore( score, highScorePosition, onRestart, onExit );
			}
			else
			{
				_GameOverPanel.ShowWithoutHighscore( score, lowestHighScoreValue.GetValueOrDefault(0), onRestart, onExit );
			}
		}
	}
}
