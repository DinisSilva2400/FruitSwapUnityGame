using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;

    [Header("Combo Settings")]
    public float comboWindow = 3f;
    public int combosRequired = 2;

    

    private int comboCount = 0;
    private float lastMatchTime = -10f;

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

        if (now - lastMatchTime <= comboWindow)
        {
            comboCount++;
        }
        else
        {
            comboCount = 1;
        }

        lastMatchTime = now;

        if (comboCount >= combosRequired)
        {
            TriggerCombo();
            comboCount = 0; // reset após mostrar
        }
    }

    void TriggerCombo()
    {
        if (visualController != null)
        {
            visualController.ShowCombo(comboCount);
        }
    }
}
