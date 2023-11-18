using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	#region Fields
	public static GameManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] protected bool dontDestroyOnLoad = true;
    [Header("Random")]
    [SerializeField] private int seed;
	[Header("Waves")]
	[SerializeField] private int startEnemies = 3;
	[SerializeField] private int incrementEnemiesPerWave = 1;
	[Header("References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text reachedText;
    [SerializeField] private TMP_Text andScoreText;
	[SerializeField] private TMP_Text continueText;
    [SerializeField] private TMP_Text startText;
	[SerializeField] private Button retryButton;
	[SerializeField] private GameObject losePanel;
	[SerializeField] private GameObject healthBar;
	[SerializeField] private GameObject lifePrefab;
	[SerializeField] private PlayerController player;
	[SerializeField] private HealthController playerHealth;
	[Header("Debug")]
	[SerializeField] private int currentWave = 0;
	[SerializeField] private int currentWaveQuantity;
	[SerializeField] private int numberOfEnemies;
	[SerializeField] private int score;
	[SerializeField] private bool isWaitingContinue;
	[SerializeField] private bool isGameOver;
	[SerializeField] private bool isGameRunning;
	#endregion

	#region Properties
	public static bool IsWaitingContinue => Instance.isWaitingContinue;
	public static bool IsGameRunning => Instance.isGameRunning;
	public static bool IsGameOver => Instance.isGameOver;
	public static int CurrentWaveQuantity => Instance.currentWaveQuantity + Instance.currentWave;
	public static int NumberOfEnemies
    {
		get => Instance.numberOfEnemies;
		set => Instance.numberOfEnemies = value;
	}
	#endregion

	#region Unity Messages
	private void Awake()
	{
        // inicializa a funçao que faz esse manager persistir em qualquer cena e ser accessado sem precisar de referencia
        InitializeSingleton();

		// coloca variaveis no default
		Random.InitState(seed);
		currentWave = 0;
		currentWaveQuantity = startEnemies;

		losePanel.SetActive(false);
		continueText.gameObject.SetActive(false);
		retryButton.onClick.AddListener(Retry);
		isGameRunning = false;
		isGameOver = false;

		// esconde o cursor do mouse
		Cursor.visible = false;
	}

	private void Start()
	{
		UpdateHealth();
		player.HidePlayer();
	}

	private void OnDestroy()
	{
		retryButton.onClick.RemoveListener(Retry);
	}

	private void Update()
	{
		// wait player to click on screen to start game
		if (!isGameOver && !isGameRunning && Input.GetButtonDown("Fire1"))
		{
			startText.gameObject.SetActive(false);
			player.ShowPlayer();
			isGameRunning = true;
		}

		// wait player to click on screen
		if (!isGameOver && isWaitingContinue && Input.GetButtonDown("Fire1"))
		{
			continueText.gameObject.SetActive(false);
			player.ShowPlayer();
			isWaitingContinue = false;
		}

		// if killed every enemy change wave and increase quantity
		if (NumberOfEnemies <= 0)
        {
			currentWave++;
			for (int i = 0; i < CurrentWaveQuantity; i++)
            {
				EnemySpawner.SpawnRandom();
            }
        }

		UpdateUI();
	}
    #endregion

    #region Public Methods
    public static void IncreaseScore(int score)
    {
        Instance.score += score;
    }

	public static void UpdateHealth()
	{
		// dinamic setup for player health bar
		if (Instance.healthBar.transform.childCount > 0)
		{
			foreach (Transform child in Instance.healthBar.transform)
				Destroy(child.gameObject);
		}

		for (int i = 0; i < Instance.playerHealth.CurrentHealth; i++)
			Instantiate(Instance.lifePrefab, Instance.healthBar.transform);
	}

	public static void WaitContinue()
	{
		Instance.isWaitingContinue = true;
		Instance.continueText.gameObject.SetActive(true);
		Instance.player.HidePlayer();
	}

	public void Retry()
    {
		isGameRunning = true;
		isGameOver = false;
		Cursor.visible = false;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void Defeat()
	{
		continueText.gameObject.SetActive(false);
		isWaitingContinue = false;
		isGameRunning = false;

		losePanel.SetActive(true);
		Cursor.visible = true;
		isGameOver = true;
	}
	#endregion

	#region Private Methods
	private void InitializeSingleton()
    {
		if (Instance == null)
		{
			Instance = this;

			if (dontDestroyOnLoad)
				DontDestroyOnLoad(gameObject);
		}
		else if (Instance != this)
		{
			Destroy(Instance.gameObject);
			Instance = this;

			if (dontDestroyOnLoad)
				DontDestroyOnLoad(gameObject);
		}
	}

    private void UpdateUI()
    {
		scoreText.text = score.ToString();
		waveText.text = currentWave.ToString();
		reachedText.text = $"You reached wave {currentWave}";
		andScoreText.text = $"And scored {score}";
	}
	#endregion
}
