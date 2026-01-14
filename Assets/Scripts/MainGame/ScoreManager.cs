using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;

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
        UpdateUI();
    }

    public void AddScore(int points)
    {
        score += points;
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
