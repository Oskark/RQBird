using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _ScoreText;

    [SerializeField] private GameObject _HighScoreContainer;
    [SerializeField] private TextMeshProUGUI _HighScorePositionText;
    
    [SerializeField] private GameObject _NoHighScoreContainer;
    [SerializeField] private TextMeshProUGUI _ToGetHighScoreValText;

    public void ShowWithHighscore( int score, int highScorePosition )
    {
        _ScoreText.text = score.ToString();
        _HighScorePositionText.text = highScorePosition.ToString();
        
        _HighScoreContainer.SetActive( true );
        _NoHighScoreContainer.SetActive( false );    
    }
    
    public void ShowWithoutHighscore( int score, int lowestHighScoreAmount )
    {
        _ScoreText.text = score.ToString();
        _ToGetHighScoreValText.text = lowestHighScoreAmount.ToString();
        
        _HighScoreContainer.SetActive( false );
        _NoHighScoreContainer.SetActive( true );
    }
}
