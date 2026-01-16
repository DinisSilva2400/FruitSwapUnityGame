using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public SpriteRenderer volumeRenderer;
    public Sprite[] volumeSprites; // 6 sprites (0 a 5)

    public GameObject PlusButton;
    public GameObject MinusButton;
    public GameObject SettingsMenu;

    private int currentLevel;
    private const int maxLevel = 5;


    //Musica On Off

    public GameObject musicButton;
    public Sprite musicOn;
    public Sprite musicOff;

    public AudioSource musicSource;

    private bool musicMuted;


    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("VolumeLevel", maxLevel);
        currentLevel = Mathf.Clamp(currentLevel, 0, maxLevel);
        SetVolume(currentLevel);

        musicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        ApplyMusicState();
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

        if (hit.collider == null)
            return;

        GameObject hitObject = hit.collider.gameObject;

        if (hitObject == PlusButton)
        {
            ChangeVolume(+1);
        }
        else if (hitObject == MinusButton)
        {
            ChangeVolume(-1);
        }
        else if (hitObject.name == "ResumeButton2")
        {
            SettingsMenu.SetActive(false);
        }
        else if (hitObject == musicButton)
        {
            ToggleMusic();
        }
    }

    void ChangeVolume(int direction)
    {
        currentLevel += direction;
        currentLevel = Mathf.Clamp(currentLevel, 0, maxLevel);
        SetVolume(currentLevel);
    }

    void SetVolume(int level)
    {
        // 0 = mute, 5 = max
        float volume = (float)level / maxLevel;

        AudioListener.volume = volume;
        volumeRenderer.sprite = volumeSprites[level];

        PlayerPrefs.SetInt("VolumeLevel", level);
        PlayerPrefs.Save();
    }

    void ToggleMusic()
    {
        musicMuted = !musicMuted;
        ApplyMusicState();

        PlayerPrefs.SetInt("MusicMuted", musicMuted ? 1 : 0);
        PlayerPrefs.Save();

    }

    void ApplyMusicState()
    {
        SpriteRenderer sr = musicButton.GetComponent<SpriteRenderer>();

        if(musicMuted)
        {
            musicSource.mute = true;
            sr.sprite = musicOff;
        }
        else
        {
            musicSource.mute = false;
            sr.sprite = musicOn;
        }
    }

}
