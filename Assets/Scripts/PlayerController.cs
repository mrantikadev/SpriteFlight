using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public UIDocument uiDocument;
    private Label scoreText;
    private Label highScoreText;
    private Button restartButton;

    [Header("Movement Properties")]
    [SerializeField] private float thrustForce = 1f;
    [SerializeField] private float maxSpeed = 5f;

    [Header("Prefab References")]
    [SerializeField] private GameObject boosterFlamePrefab;
    [SerializeField] private GameObject explosionEffectPrefab;

    private float elapsedTime = 0f;

    private float score = 0f;
    private float scoreMultiplier = 10f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");

        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;

        LoadHighScoreData();
    }

    private void Update()
    {
        UpdateScore();
        MovePlayer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameOver();
    }

    private void UpdateScore()
    {
        elapsedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
        scoreText.text = "Score: " + score;
    }

    private void MovePlayer()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            // Calculate mouse direction
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            Vector2 direction = (mousePos - transform.position).normalized;

            // Move player in direction of mouse
            transform.up = direction;
            rb.AddForce(direction * thrustForce);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            boosterFlamePrefab.SetActive(true);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            boosterFlamePrefab.SetActive(false);
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SaveHighScoreData()
    {
        PlayerPrefs.SetFloat("HighScore", score);
        PlayerPrefs.Save();
    }

    private void LoadHighScoreData()
    {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            float highScore = PlayerPrefs.GetFloat("HighScore");
            highScoreText.text = "High Score: " + highScore;
        }
    }

    private void GameOver()
    {
        Destroy(gameObject);
        Instantiate(explosionEffectPrefab, transform.position, transform.rotation);
        restartButton.style.display = DisplayStyle.Flex;

        if (score > PlayerPrefs.GetFloat("HighScore", 0f))
        {
            PlayerPrefs.SetFloat("HighScore", score);
            highScoreText.text = "High Score: " + score;
            SaveHighScoreData();
        }
    }
}
