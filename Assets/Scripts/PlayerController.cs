using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public InputAction moveForward;
    public InputAction lookPosition;

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
    [SerializeField] private GameObject borderParent;
    [SerializeField] private GameObject obstacleParent;

    private float elapsedTime = 0f;

    private float score = 0f;
    private float scoreMultiplier = 10f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");
        highScoreText.style.display = DisplayStyle.None;

        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;

        moveForward.Enable();
        lookPosition.Enable();

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
        if (moveForward.IsPressed())
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(lookPosition.ReadValue<Vector2>());
            Vector2 direction = (mousePos - transform.position).normalized;

            // Move player in direction of mouse
            transform.up = direction;
            rb.AddForce(direction * thrustForce);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        if (moveForward.WasPressedThisFrame())
        {
            boosterFlamePrefab.SetActive(true);
        }
        else if (moveForward.WasReleasedThisFrame())
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
        highScoreText.style.display = DisplayStyle.Flex;
        restartButton.style.display = DisplayStyle.Flex;
        borderParent.SetActive(false);
        Destroy(obstacleParent, 10f);

        if (score > PlayerPrefs.GetFloat("HighScore", 0f))
        {
            PlayerPrefs.SetFloat("HighScore", score);
            highScoreText.text = "High Score: " + score;
            SaveHighScoreData();
        }
    }
}
