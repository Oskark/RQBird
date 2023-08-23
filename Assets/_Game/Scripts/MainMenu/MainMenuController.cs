using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] private GameObject _MainMenuPanel;
    [SerializeField] private GameObject _HighScoresPanel;

    [SerializeField] private HighScoresDisplayer _HighScoresDisplayer;

    [Inject] private GameplayPreloader _gameplayPreloader;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        _gameplayPreloader.BeginPreloading(  );
    }

    public void Evt_StartGame()
    {
        _gameplayPreloader.MakeSurePreloadCompletedAndThen( LoadGameplay);

        return;
        
        void LoadGameplay()
        {
            SceneManager.LoadScene( "Gameplay" );
        }
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
