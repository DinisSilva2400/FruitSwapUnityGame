using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonUI : MonoBehaviour, IPointerClickHandler
{
    public enum ButtonType
    {
        Resume,
        MainMenu,
        Exit,
        Settings,
        BackFromSettings
    }


    public ButtonType buttonType;
    public PauseMenuManager pauseManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case ButtonType.Resume:
                pauseManager.Resume();
                break;

            case ButtonType.MainMenu:
                pauseManager.GoToMainMenu();
                break;

            case ButtonType.Exit:
                pauseManager.ExitGame();
                break;

            case ButtonType.Settings:
                pauseManager.OpenSettings();
                break;

            case ButtonType.BackFromSettings:
                pauseManager.CloseSettings();
                break;
        }
    }
}
