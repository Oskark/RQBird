
using System;
using UnityEngine;

namespace Gameplay.UI
{
	public class GameplayHUD : MonoBehaviour
	{
		[SerializeField] private GameOverPanel _GameOverPanel;
		
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
