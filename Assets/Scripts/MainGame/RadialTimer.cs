using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RadialTimer : MonoBehaviour
{
    public Image timerImage;
    public float totalTime = 180f;
    public string sceneToLoad = "GameOver";

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
        SceneManager.LoadScene(sceneToLoad);
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
