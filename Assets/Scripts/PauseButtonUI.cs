using UnityEngine;
using UnityEngine.EventSystems;

public class PauseButtonUI : MonoBehaviour, IPointerClickHandler
{
    public PauseMenuManager pauseManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        pauseManager.TogglePause();
    }
}
