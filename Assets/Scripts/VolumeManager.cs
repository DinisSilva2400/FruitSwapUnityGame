using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public SpriteRenderer volumeRenderer;
    public Sprite[] volumeSprites; // 5 sprites

    public GameObject PlusButton;
    public GameObject MinusButton;
    public GameObject SettingsMenu;

    private int currentLevel;
    private const int maxLevel = 5;

    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("VolumeLevel", maxLevel);
        SetVolume(currentLevel);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);


            string name = hit.collider.gameObject.name;            
            if (hit.collider == null) return;

            if (hit.collider.gameObject == PlusButton)
            {
                ChangeVolume(+1);
            }
            else if (hit.collider.gameObject == MinusButton)
            {
                ChangeVolume(-1);
            }
            else if(name == "ResumeButton2")
            {
               SettingsMenu.SetActive(false); 
            }

        }
    }

    void ChangeVolume(int direction)
    {
        currentLevel += direction;

        currentLevel = Mathf.Clamp(currentLevel, 1, maxLevel);

        SetVolume(currentLevel);
    }

    void SetVolume(int level)
    {
        float volume = (float)level / maxLevel;

        AudioListener.volume = volume;
        volumeRenderer.sprite = volumeSprites[level - 1];

        PlayerPrefs.SetInt("VolumeLevel", level);
        PlayerPrefs.Save();
    }
}
