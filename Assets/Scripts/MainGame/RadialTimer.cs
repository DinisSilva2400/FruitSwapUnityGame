using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RadialTimer : MonoBehaviour
{
    public Image timerImage;
    public float totalTime = 180f;
    public string sceneToLoad = "Menu";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip timeWarningSound; // Som que toca quando faltam 10 segundos

    private float timeLeft;
    private bool hasPlayedWarning = false; // Garante que o som toca apenas uma vez
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

        // Tocar som de aviso quando faltam 10 segundos
        if (timeLeft <= 10f && !hasPlayedWarning)
        {
            hasPlayedWarning = true;
            if (audioSource != null && timeWarningSound != null)
            {
                audioSource.PlayOneShot(timeWarningSound);
            }
        }

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
        SceneManager.LoadScene("EndGame");
    }

    public void AddTime(float seconds)
    {
        timeLeft += seconds;
        if (timeLeft > totalTime)
        {
            timeLeft = totalTime;
        }

        // Se voltarmos para cima de 10 segundos, reseta a flag
        if (timeLeft > 10f)
        {
            hasPlayedWarning = false;
        }
    }
}
