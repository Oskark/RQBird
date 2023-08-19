using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] private GameObject _MainMenuPanel;
    [SerializeField] private GameObject _HighScoresPanel;

    [SerializeField] private HighScoresDisplayer _HighScoresDisplayer;
    
    public void Evt_StartGame()
    {
        SceneManager.LoadScene( "Gameplay" );
        SceneManager.UnloadSceneAsync( "MainMenu" );
    }
    
    public void Evt_QuitGame()
    {
        Application.Quit();
    }

    public void Evt_OpenHighScores()
    {
        _MainMenuPanel.gameObject.SetActive( false );
        _HighScoresPanel.gameObject.SetActive( true );
        
        _HighScoresDisplayer.Show();
    }

    public void Evt_OpenMainMenu()
    {
        _MainMenuPanel.gameObject.SetActive( true );
        _HighScoresPanel.gameObject.SetActive( false );
    }
}
