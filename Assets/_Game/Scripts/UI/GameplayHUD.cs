
using UnityEngine;

namespace Gameplay.UI
{
	public class GameplayHUD : MonoBehaviour
	{
		[SerializeField] private GameOverPanel _GameOverPanel;
		
		public void ShowGameOverPanel( int score, int highScorePosition, int? lowestHighScoreValue )
		{
			_GameOverPanel.gameObject.SetActive( true );
			
			if ( highScorePosition > 0 )
			{
				_GameOverPanel.ShowWithHighscore( score, highScorePosition );
			}
			else
			{
				_GameOverPanel.ShowWithoutHighscore( score, lowestHighScoreValue.GetValueOrDefault(0) );
			}
		}
	}
}
