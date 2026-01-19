using UnityEngine;
using UnityEngine.EventSystems;

public class SoundButtonUI : MonoBehaviour, IPointerClickHandler
{
    public enum SoundButtonType
    {
        Plus,
        Minus
    }

    public SoundButtonType buttonType;
    public SoundSettingsUI soundSettings;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (buttonType == SoundButtonType.Plus)
            soundSettings.IncreaseSound();
        else
            soundSettings.DecreaseSound();
    }
}
