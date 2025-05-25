using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private float countdownStartingValue;
    [SerializeField] private TMP_Text countdownText;

    [SerializeField, Space] private GameObject endLevelPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TMP_Text totalMoneyText;
    [SerializeField] private Animator heistStartedAnim;
    [SerializeField] private Button restartButton, quitButton;

    [SerializeField, Space] private AudioSource policeSirenAudio;
    [SerializeField] private AudioSource heistStartAlarm;

    private int totalMoneyAmount;

    private float timer;

    private bool countdownHasStarted;
    private bool levelHasEnded;

    private Van van;

    public bool CountdownHasStarted { get { return countdownHasStarted; }}

    public static LevelManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        timer = countdownStartingValue;

        SetCountdownText();

        restartButton.onClick.AddListener(Restart);
        quitButton.onClick.AddListener(Quit);
    }

    private void Start()
    {
        van = FindFirstObjectByType<Van>();
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

        timer -= Time.deltaTime;

        SetCountdownText();

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

        PlayerManager.Wrapping.StopWrapping();
        PlayerManager.Movement.enabled = false;
        PlayerManager.Interaction.enabled = false;

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
