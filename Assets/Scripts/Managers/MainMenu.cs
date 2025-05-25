using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        // Initialize buttons
        playButton.onClick.AddListener(OnPlayClick);
        quitButton.onClick.AddListener(OnQuitClick);
    }

    public void OnPlayClick()
    {
        playButton.enabled = false;
        SceneManager.LoadScene("Game");
    }

    public void OnQuitClick()
    {
        quitButton.enabled = false;
        Application.Quit();
    }
}
