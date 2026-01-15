using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public int highScore = 0;

    [Tooltip("Arrasta aqui o TextMeshProUGUI do high score")]
    public TextMeshProUGUI HighScore;

    [Tooltip("Arrasta aqui o TextMeshProUGUI do score")]
    public TextMeshProUGUI scoreText;

    [Tooltip("Texto antes do número, ex: 'Score: '")]
    public string prefix = "Score: ";

    [Tooltip("Texto depois do número")]
    public string suffix = "";

    [Header("Particles")]
    [Tooltip("Particle System da explosão de score")]
    public ParticleSystem scoreParticles;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0); // carrega high score
        UpdateUI();
    }

    public void AddScore(int points)
    {
        score += points;
        highScore = score;
        UpdateUI();
        PlayParticles();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = prefix + score.ToString() + suffix;

        if (HighScore != null)
            HighScore.text = "High Score: " + highScore.ToString();
    }

    void PlayParticles()
    {
        if (scoreParticles != null)
        {
            scoreParticles.Stop();
            scoreParticles.Play();
        }
    }

    public void EndLevel()
    {
        
        if(score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        UpdateUI(); // atualiza a UI com o novo high score
        ResetScore(); // opcional: zera o score atual para começar de novo
    }
}
