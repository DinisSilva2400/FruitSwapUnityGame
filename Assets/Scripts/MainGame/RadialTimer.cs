using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RadialTimer : MonoBehaviour
{
    public Image timerImage;
    public float totalTime = 180f;
    public string sceneToLoad = "Menu";

    private float timeLeft;
    public static RadialTimer Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        timeLeft = totalTime;
        timerImage.fillAmount = 1f;
    }

    void Update()
    {
        if (timeLeft <= 0)
            return;

        timeLeft -= Time.deltaTime;
        timerImage.fillAmount = timeLeft / totalTime;

        if (timeLeft <= 0)
        {
            timeLeft = 0;
            TimerEnded();
        }
    }

    void TimerEnded()
    {
        // Guarda o high score antes de mudar de cena
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.EndLevel();
        }

        // Carrega a cena "Menu"
        SceneManager.LoadScene("Menu");
    }

    public void AddTime(float seconds)
    {
        timeLeft += seconds;
        if (timeLeft > totalTime)
        {
            timeLeft = totalTime;
        }
    }
}
