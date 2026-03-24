using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;

    [Header("Combo Settings")]
    public float comboWindow = 3f;
    public float comboWindowExtended = 3f; // 4 segundos para 2º e 3º combo
    public int combosRequired = 2;

    private int comboCount = 0;
    private float comboWindowStartTime = -10f;
    private int currentComboLevel = 0; // Nível atual do combo (1, 2, 3...)
    private float lastComboShowTime = -10f; // Quando foi mostrado o último combo

    private int pointsMultiplier = 1; // Multiplicador de pontos (1, 2, 3)
    private float multiplierEndTime = -10f; // Quando expira o multiplicador

    [Header("Visual")]
    public ComboVisualController visualController;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterMatch()
    {
        float now = Time.time;

        // Determinar a duração da janela: 3s para o 1º combo, 4s para os seguintes
        float currentWindowDuration = (currentComboLevel == 0) ? comboWindow : comboWindowExtended;

        // Se passou mais de X segundos desde o início da janela de tempo, resetar para zero
        if (comboCount > 0 && now - comboWindowStartTime > currentWindowDuration)
        {
            // Janela expirou sem conseguir 2 junções
            comboCount = 0;
            currentComboLevel = 0;
        }

        // Se é a primeira combinação dessa janela
        if (comboCount == 0)
        {
            comboWindowStartTime = now;
        }

        // Incrementar contador de combinações
        comboCount++;

        // Se atingiu 2 junções dentro da janela
        if (comboCount >= combosRequired)
        {
            currentComboLevel++;
            lastComboShowTime = now;
            TriggerCombo(currentComboLevel);
            comboCount = 0; // Reset para contar as próximas 2 junções
        }
    }

    void TriggerCombo(int level)
    {
        // Combo Level 1 = x2 pontos | Combo Level 2+ = x3 pontos
        pointsMultiplier = (level == 1) ? 2 : 3;
        multiplierEndTime = Time.time + comboWindow;

        if (visualController != null)
        {
            visualController.ShowCombo(level);
        }
    }

    // Retorna o multiplicador de pontos atual
    public int GetPointsMultiplier()
    {
        // Se o multiplicador expirou, volta ao normal (x1)
        if (Time.time > multiplierEndTime)
        {
            pointsMultiplier = 1;
        }
        return pointsMultiplier;
    }
}
