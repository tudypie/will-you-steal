using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject endLevelPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TMP_Text totalMoneyText;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Button restartButton, quitButton;
    [SerializeField] private Animator heistStartedAnim;
    [SerializeField] private AudioSource policeSirenAudio;
    [SerializeField] private AudioSource heistStartAlarm;

    [Header("Settings")]
    [SerializeField] private float countdownStartingValue;

    private int totalMoneyAmount;

    private float timer;

    private bool countdownHasStarted;
    private bool levelHasEnded;

    private Van van;

    public bool CountdownHasStarted { get { return countdownHasStarted; }}

    public static LevelManager Instance;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        van = FindFirstObjectByType<Van>();

        // Initialize timer
        timer = countdownStartingValue;
        SetCountdownText();

        // Initialize buttons
        restartButton.onClick.AddListener(Restart);
        quitButton.onClick.AddListener(Quit);
    }

    private void Update()
    {
        if (levelHasEnded) { return; }

        UpdateCountdown();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    private void UpdateCountdown()
    {
        if (!countdownHasStarted) { return; }

        // Update timer
        timer -= Time.deltaTime;
        SetCountdownText();

        // Make police siren volume increase exponentially
        float t = 1f - (timer / countdownStartingValue);
        policeSirenAudio.volume = 0.5f * Mathf.Pow(t, 3);

        if (timer <= 0)
        {
            EndLevel();
        }
    }

    private void SetCountdownText()
    {
        countdownText.text = "Police ETA: " + timer.ToString("00") + "s";
    }

    private void EndLevel()
    {
        levelHasEnded = true;

        // Disable player scripts
        PlayerManager.Wrapping.StopWrapping();
        PlayerManager.Movement.enabled = false;
        PlayerManager.Interaction.enabled = false;

        // Win or lose condition
        if (van.PlayerIsNearVan)
        {
            winPanel.SetActive(true);
            totalMoneyText.text = $"{totalMoneyAmount} $";
        }
        else
        {
            losePanel.SetActive(true);
        }

        endLevelPanel.SetActive(true);
    }

    public void StartCountdown()
    {
        if (CountdownHasStarted) { return; }

        countdownHasStarted = true;
        heistStartedAnim.enabled = true;

        policeSirenAudio.Play();
        heistStartAlarm.Play();
    }

    public void AddMoney(int amount)
    {
        totalMoneyAmount += amount;
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
