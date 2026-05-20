using System.Collections;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private AudioClip levelMusic;
    [SerializeField] private Slider progressBar;
    [SerializeField] private GameObject transitionsContainer;
    [SerializeField] private int finalLevelIndex;

    private SceneTransition[] transitions;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        transitions = transitionsContainer.GetComponentsInChildren<SceneTransition>();
        AudioManager.Instance.PlayMusic(levelMusic, fadeIn: true);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameManager.Instance?.BindUIReferences();
    }


    public void LoadScene(string sceneName, string transitionName)
    {
        DontDestroyOnLoad(progressBar.transform.root.gameObject);
        StartCoroutine(LoadSceneAsync(sceneName, transitionName));
    }

    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        Debug.Log($"Current Level: {currentIndex}, Next Level: {nextIndex}");

        if (nextIndex < finalLevelIndex)
        {
            string nextScenePath = SceneUtility.GetScenePathByBuildIndex(nextIndex);
            string nextSceneName = Path.GetFileNameWithoutExtension(nextScenePath);

            LoadScene(nextSceneName, "CrossFade");
        }
        else
        {
            GameManager.showCompletionPopup = true;
            LoadScene("MainMenu", "CrossFade");
        }
    }



    private IEnumerator LoadSceneAsync(string sceneName, string transitionName)
    {
        SceneTransition transition = transitions.First(t => t.name == transitionName);

        yield return transition.AnimateTransitionIn();

        progressBar.gameObject.SetActive(true);
        yield return null;

        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        while (scene.progress < 0.9f)
        {
            progressBar.value = Mathf.Clamp01(scene.progress / 0.9f);
            yield return null;
        }

        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            progressBar.value = Mathf.Lerp(progressBar.value, 1f, timer);
            yield return null;
        }

        scene.allowSceneActivation = true;

        progressBar.gameObject.SetActive(false);

        yield return transition.AnimateTransitionOut();
    }

}
