using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.scoreText = scoreText;
            ScoreManager.Instance.highScoreText = highScoreText;
            ScoreManager.Instance.UpdateUI();
        }
    }
}
