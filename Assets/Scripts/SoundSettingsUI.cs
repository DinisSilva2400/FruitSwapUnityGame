using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image soundBarImage;        // barra do volume geral
    public Sprite[] soundLevelSprites; // sprites 0-5
    public Image musicToggleButton;    // sprite do botão de música
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    [Header("Audio")]
    public AudioSource musicAudioSource; // o AudioSource da música

    private int soundLevel = 3;     // 0-5
    private bool musicMuted = false;

    void Start()
    {
        UpdateSoundBar();
        musicToggleButton.sprite = musicOnSprite;
    }

    // ------------------ VOLUME GERAL ------------------
    public void IncreaseSound()
    {
        soundLevel = Mathf.Clamp(soundLevel + 1, 0, 5);
        UpdateSoundBar();
    }

    public void DecreaseSound()
    {
        soundLevel = Mathf.Clamp(soundLevel - 1, 0, 5);
        UpdateSoundBar();
    }

    void UpdateSoundBar()
    {
        soundBarImage.sprite = soundLevelSprites[soundLevel];

        // Volume geral do jogo
        AudioListener.volume = soundLevel / 5f;
    }

    // ------------------ TOGGLE MÚSICA ------------------
    public void ToggleMusic()
    {
        musicMuted = !musicMuted;
        musicAudioSource.mute = musicMuted;
        musicToggleButton.sprite = musicMuted ? musicOffSprite : musicOnSprite;
    }
}
