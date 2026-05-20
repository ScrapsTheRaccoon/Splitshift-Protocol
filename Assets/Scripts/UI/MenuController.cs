using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Animator settingsMenuAnim;
    [SerializeField] private Animator pauseMenuAnim;

    private Animator currentOpenMenu = null;

    public void OpenMenu(Animator menuAnim)
    {
        // If the menu you want is already open them do nothing
        if (currentOpenMenu == menuAnim) return;

        // If a menu is currently open cllose it
        if (currentOpenMenu != null)
        {
            currentOpenMenu.SetTrigger("PanelClose");
        }

        // Open the new menu
        menuAnim.SetTrigger("PanelOpen");
        currentOpenMenu = menuAnim;
    }

    public void CloseCurrentMenu()
    {
        if (currentOpenMenu == null) return;

        currentOpenMenu.SetTrigger("PanelClose");
        currentOpenMenu = null;
    }

    public void ClosePauseMenu()
    {
        CloseCurrentMenu();
        GameManager.Instance.ResumeGame();
    }

    public void Restart()
    {
        GameManager.Instance.Restart();
    }

    public void ReturnToMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }

    public void OpenSettings()
    {
        OpenMenu(settingsMenuAnim);
    }

    public void OpenPauseMenu()
    {
        OpenMenu(pauseMenuAnim);
    }
}
