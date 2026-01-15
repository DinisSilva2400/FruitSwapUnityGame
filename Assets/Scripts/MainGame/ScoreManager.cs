using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score;
    public int highScore;

    [HideInInspector] public TextMeshProUGUI scoreText;
    [HideInInspector] public TextMeshProUGUI highScoreText;

    public string prefix = "Score: ";
    public string suffix = "";

    public ParticleSystem scoreParticles;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
        PlayParticles();
    }

    public void EndLevel()
    {
        // Só atualiza se bater recorde
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        score = 0;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = prefix + score + suffix;

        if (highScoreText != null){
            if(score > highScore){
                highScoreText.text = "High Score: " + score;
            }
            else
            {
                highScoreText.text = "High Score: " + highScore;
            }
        }

            
    }

    void PlayParticles()
    {
        if (scoreParticles != null)
        {
            scoreParticles.Stop();
            scoreParticles.Play();
        }
    }
}
