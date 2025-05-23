using System;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private float countdownStartingValue;
    [SerializeField] private TMP_Text countdownText;

    [SerializeField, Space] private GameObject endLevelPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TMP_Text totalMoneyText;

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

        countdownText.text = timer.ToString("00") + "s";
    }

    private void Start()
    {
        van = FindFirstObjectByType<Van>();
    }

    private void Update()
    {
        if (levelHasEnded) { return; }

        UpdateCountdown();
    }

    private void UpdateCountdown()
    {
        if (!countdownHasStarted) { return; }

        timer -= Time.deltaTime;

        countdownText.text = timer.ToString("00") + "s";

        if (timer <= 0)
        {
            EndLevel();
        }
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
        countdownHasStarted = true;
    }

    public void AddMoney(int amount)
    {
        totalMoneyAmount += amount;
    }
}
