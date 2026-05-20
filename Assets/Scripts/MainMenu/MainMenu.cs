using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject subtitle;
    [SerializeField] private Animator buttonAnim;
    [SerializeField] private Animator titleAnim;

    [SerializeField] private Animator levelMenuAnim;
    [SerializeField] private Animator settingMenuAnim;
    [SerializeField] private Animator exitConfirmationMenuAnim;
    [SerializeField] private Animator completionPopupAnim;

    private Animator currentOpenMenu = null;

    private void Start()
    {
        if (GameManager.showCompletionPopup)
        {
            OpenMenu(completionPopupAnim);
            GameManager.showCompletionPopup = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // TODO: Check tutorial completion status here
            subtitle.SetActive(false);
            buttonAnim.SetTrigger("buttonsUp");
        }
    }

    public void OpenMenu(Animator menuAnim)
    {
        if (currentOpenMenu != null) return;

        menuAnim.SetTrigger("PanelOpen");
        buttonAnim.SetTrigger("buttonsDown");
        titleAnim.SetTrigger("zoomOut");
        currentOpenMenu = menuAnim;
    }

    public void CloseCurrentMenu()
    {
        if (currentOpenMenu == null) return;

        currentOpenMenu.SetTrigger("PanelClose");
        buttonAnim.SetTrigger("buttonsUp");
        titleAnim.SetTrigger("zoomIn");
        currentOpenMenu = null;
    }

    public void OpenLevelMenu() => OpenMenu(levelMenuAnim);
    public void CloseLevelMenu() => CloseCurrentMenu();

    public void OpenSettingsMenu() => OpenMenu(settingMenuAnim);
    public void CloseSettingsMenu() => CloseCurrentMenu();

    public void OpenExitConfirmationPanel() => OpenMenu(exitConfirmationMenuAnim);
    public void CloseExitConfirmationPanel() => CloseCurrentMenu();

    public void QuitToDesktop()
    {
        Application.Quit();
    }

    public void playLevelOne()
    {
        LevelManager.Instance.LoadScene("Level1", "CircleWipe");
    }

    public void loadCredits()
    {
        LevelManager.Instance.LoadScene("Credits", "CrossFade");
    }
}
