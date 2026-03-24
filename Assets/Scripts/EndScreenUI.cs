using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScreenUI : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public Image scoreBackground; // A tábua que fica atrás dos pontos
    public Image menuButton; // Botão para ir ao menu
    public Image gameButton; // Botão para voltar ao jogo

    public float slideDistance = 300f;   // distância que vem de cima
    public float slideDuration = 0.6f;   // tempo da animação de entrada
    public float countDuration = 2f;     // tempo da contagem dos pontos
    public float delayBeforeCount = 0.3f; // espera antes de começar a contar
    public float buttonSlideDistance = 1500f; // distância que os botões vêm de baixo (fora da tela)
    public float buttonSlideDuration = 0.6f; // tempo da animação dos botões

    private RectTransform rectTransform;
    private RectTransform backgroundRectTransform;
    private RectTransform textRectTransform;
    private RectTransform highScoreRectTransform;
    private RectTransform menuButtonRectTransform;
    private RectTransform gameButtonRectTransform;
    private Vector2 targetPosition;
    private Vector2 backgroundTargetPosition;
    private Vector2 textTargetPosition;
    private Vector2 highScoreTargetPosition;
    private Vector2 menuButtonTargetPosition;
    private Vector2 gameButtonTargetPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Guarda a posição final (onde está no editor)
        targetPosition = rectTransform.anchoredPosition;

        // Começa acima do ecrã
        rectTransform.anchoredPosition = new Vector2(targetPosition.x, targetPosition.y + slideDistance);

        // Configura a tábua (background) se existir
        if (scoreBackground != null)
        {
            backgroundRectTransform = scoreBackground.GetComponent<RectTransform>();
            backgroundTargetPosition = backgroundRectTransform.anchoredPosition;
            backgroundRectTransform.anchoredPosition = new Vector2(backgroundTargetPosition.x, backgroundTargetPosition.y + slideDistance);
        }

        // Configura o texto se existir
        if (finalScoreText != null)
        {
            textRectTransform = finalScoreText.GetComponent<RectTransform>();
            textTargetPosition = textRectTransform.anchoredPosition;
            textRectTransform.anchoredPosition = new Vector2(textTargetPosition.x, textTargetPosition.y + slideDistance);
            finalScoreText.text = "Score: 0";
        }

        // Configura o high score se existir
        if (highScoreText != null)
        {
            highScoreRectTransform = highScoreText.GetComponent<RectTransform>();
            highScoreTargetPosition = highScoreRectTransform.anchoredPosition;
            highScoreRectTransform.anchoredPosition = new Vector2(highScoreTargetPosition.x, highScoreTargetPosition.y + slideDistance);
        }

        // Configura os botões se existirem
        if (menuButton != null)
        {
            menuButtonRectTransform = menuButton.GetComponent<RectTransform>();
            menuButtonTargetPosition = menuButtonRectTransform.anchoredPosition;
            menuButtonRectTransform.anchoredPosition = new Vector2(menuButtonTargetPosition.x, menuButtonTargetPosition.y - buttonSlideDistance);
            
            Button menuBtnComponent = menuButton.GetComponent<Button>();
            if (menuBtnComponent != null)
            {
                menuBtnComponent.onClick.AddListener(GoToMenu);
                Debug.Log("Menu button listener added");
            }
            else
            {
                Debug.LogError("Menu button doesn't have Button component!");
            }
        }

        if (gameButton != null)
        {
            gameButtonRectTransform = gameButton.GetComponent<RectTransform>();
            gameButtonTargetPosition = gameButtonRectTransform.anchoredPosition;
            gameButtonRectTransform.anchoredPosition = new Vector2(gameButtonTargetPosition.x, gameButtonTargetPosition.y - buttonSlideDistance);
            
            Button gameBtnComponent = gameButton.GetComponent<Button>();
            if (gameBtnComponent != null)
            {
                gameBtnComponent.onClick.AddListener(GoToGame);
                Debug.Log("Game button listener added");
            }
            else
            {
                Debug.LogError("Game button doesn't have Button component!");
            }
        }

        if (ScoreManager.Instance != null)
        {
            if (highScoreText != null)
                highScoreText.text = "High Score: " + ScoreManager.Instance.highScore;
            
            if (finalScoreText != null)
            {
                StartCoroutine(AnimateEntry());
            }
        }
        else
        {
            Debug.LogError("ScoreManager not found!");
        }
    }

    IEnumerator AnimateEntry()
    {
        // Animação de slide para baixo (texto)
        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 startBackgroundPos = Vector2.zero;
        Vector2 startTextPos = Vector2.zero;
        Vector2 startHighScorePos = Vector2.zero;

        if (backgroundRectTransform != null)
        {
            startBackgroundPos = backgroundRectTransform.anchoredPosition;
        }

        if (textRectTransform != null)
        {
            startTextPos = textRectTransform.anchoredPosition;
        }

        if (highScoreRectTransform != null)
        {
            startHighScorePos = highScoreRectTransform.anchoredPosition;
        }

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPosition, t);
            
            // Anima a tábua também
            if (backgroundRectTransform != null)
            {
                backgroundRectTransform.anchoredPosition = Vector2.Lerp(startBackgroundPos, backgroundTargetPosition, t);
            }

            // Anima o texto também
            if (textRectTransform != null)
            {
                textRectTransform.anchoredPosition = Vector2.Lerp(startTextPos, textTargetPosition, t);
            }

            // Anima o high score também
            if (highScoreRectTransform != null)
            {
                highScoreRectTransform.anchoredPosition = Vector2.Lerp(startHighScorePos, highScoreTargetPosition, t);
            }

            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
        if (backgroundRectTransform != null)
        {
            backgroundRectTransform.anchoredPosition = backgroundTargetPosition;
        }
        if (textRectTransform != null)
        {
            textRectTransform.anchoredPosition = textTargetPosition;
        }
        if (highScoreRectTransform != null)
        {
            highScoreRectTransform.anchoredPosition = highScoreTargetPosition;
        }

        // Pequena pausa antes de começar a contar
        yield return new WaitForSeconds(delayBeforeCount);

        // Contagem dos pontos
        StartCoroutine(CountScore());
    }

    IEnumerator CountScore()
    {
        int finalScore = ScoreManager.Instance.finalScore;
        float elapsed = 0f;

        while (elapsed < countDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / countDuration);
            int currentScore = Mathf.RoundToInt(Mathf.Lerp(0, finalScore, t));
            finalScoreText.text = "Score: " + currentScore;
            yield return null;
        }

        // Garante que termina no valor exato
        finalScoreText.text = "Score: " + finalScore;

        // Anima os botões depois
        StartCoroutine(AnimateButtons());
    }

    IEnumerator AnimateButtons()
    {
        float elapsed = 0f;
        Vector2 startMenuPos = Vector2.zero;
        Vector2 startGamePos = Vector2.zero;

        if (menuButtonRectTransform != null)
        {
            startMenuPos = menuButtonRectTransform.anchoredPosition;
        }

        if (gameButtonRectTransform != null)
        {
            startGamePos = gameButtonRectTransform.anchoredPosition;
        }

        while (elapsed < buttonSlideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / buttonSlideDuration);

            if (menuButtonRectTransform != null)
            {
                menuButtonRectTransform.anchoredPosition = Vector2.Lerp(startMenuPos, menuButtonTargetPosition, t);
            }

            if (gameButtonRectTransform != null)
            {
                gameButtonRectTransform.anchoredPosition = Vector2.Lerp(startGamePos, gameButtonTargetPosition, t);
            }

            yield return null;
        }

        if (menuButtonRectTransform != null)
        {
            menuButtonRectTransform.anchoredPosition = menuButtonTargetPosition;
        }

        if (gameButtonRectTransform != null)
        {
            gameButtonRectTransform.anchoredPosition = gameButtonTargetPosition;
        }
    }

    void GoToMenu()
    {
        Debug.Log("Going to Menu!");
        SceneManager.LoadScene("menu");
    }

    void GoToGame()
    {
        Debug.Log("Going to Game!");
        SceneManager.LoadScene("SampleScene");
    }
}