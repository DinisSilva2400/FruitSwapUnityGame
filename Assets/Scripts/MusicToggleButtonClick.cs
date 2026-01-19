using UnityEngine;
using UnityEngine.EventSystems;

public class MusicToggleButtonClick : MonoBehaviour, IPointerClickHandler
{
    public SoundSettingsUI soundSettings; // referencia ao script principal do menu

    public void OnPointerClick(PointerEventData eventData)
    {
        soundSettings.ToggleMusic(); // chama o toggle do som
    }
}
