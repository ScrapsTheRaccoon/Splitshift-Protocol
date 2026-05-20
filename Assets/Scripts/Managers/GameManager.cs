using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool isPaused = false;
    private bool isGameOver = false;

    public static bool showCompletionPopup = false;
    public bool IsPaused => isPaused;
    public bool IsGameOver => isGameOver;

    [SerializeField] private AudioClip gameOverAudioClip;
    [SerializeField] private AudioClip pauseClip;
    [SerializeField] private AudioClip unpauseClip;

    private MenuController menuController;
    private Animator pauseMenuAnim;
    private Animator settingsMenuAnim;
    private Animator gameOverMenuAnim;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void BindUIReferences()
    {
        menuController = FindAnyObjectByType<MenuController>();
        pauseMenuAnim = GameObject.Find("PauseMenu")?.GetComponent<Animator>();
        settingsMenuAnim = GameObject.Find("SettingsMenu")?.GetComponent<Animator>();
        gameOverMenuAnim = GameObject.Find("GameOverMenu")?.GetComponent<Animator>();
    }

    private void Update()
    {
        if (isGameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            AudioManager.Instance.PlaySFX(pauseClip);
            menuController.OpenMenu(pauseMenuAnim);
        }
        else
        {
            AudioManager.Instance.PlaySFX(unpauseClip);
            menuController.CloseCurrentMenu();
        }

        Debug.Log("Pause State: " + isPaused);
    }

    public void openSettingsMenu()
    {
        menuController.CloseCurrentMenu();
        menuController.OpenMenu(settingsMenuAnim);
    }

    public void closeSettingsMenu()
    {
        menuController.CloseCurrentMenu();
        menuController.OpenMenu(pauseMenuAnim);

        TogglePause();
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        StartCoroutine(GameOver());
        
    }

    IEnumerator GameOver()
    {
        isGameOver = true;
        AudioManager.Instance.FadeOutMusic();
        AudioManager.Instance.PlaySFX(gameOverAudioClip);

        yield return new WaitForSeconds(2f);
        Time.timeScale = 0f;

        menuController.OpenMenu(gameOverMenuAnim);
    }

    public void CompleteLevel()
    {
        LevelManager.Instance.LoadNextLevel();
        AudioManager.Instance.FadeOutMusic();
        AudioManager.Instance.FadeOutSFX();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        LevelManager.Instance.LoadScene( "MainMenu", "CrossFade");
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

