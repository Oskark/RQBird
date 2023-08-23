using System;
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


    private Action _onRestart;
    private Action _onExit;
    
    public void ShowWithHighscore( int score, int highScorePosition, Action onRestart, Action onExit )
    {
        _ScoreText.text = score.ToString();
        _HighScorePositionText.text = (highScorePosition + 1).ToString();
        
        _HighScoreContainer.SetActive( true );
        _NoHighScoreContainer.SetActive( false );    
        
        _onRestart = onRestart;
        _onExit = onExit;
    }
    
    public void ShowWithoutHighscore( int score, int lowestHighScoreAmount, Action onRestart, Action onExit )
    {
        _ScoreText.text = score.ToString();
        _ToGetHighScoreValText.text = lowestHighScoreAmount.ToString();
        
        _HighScoreContainer.SetActive( false );
        _NoHighScoreContainer.SetActive( true );
        
        _onRestart = onRestart;
        _onExit = onExit;
    }
    
    public void Evt_OnRestart()
    {
        _onRestart?.Invoke();
    }

    public void Evt_OnExit()
    {
        _onExit?.Invoke();
    }
}
