using UnityEngine;

public class TopOverlayScaler : MonoBehaviour
{
    [Header("Offset em percentagem do ecrã")]
    [Range(0f, 1f)]
    public float verticalOffset = 0.9f; // 90% da altura (topo)

    void Start()
    {
        Camera cam = Camera.main;

        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * Screen.width / Screen.height;

        // Posicionar no topo, centrado
        transform.position = new Vector3(
            0f,
            camHeight * (verticalOffset - 0.5f),
            transform.position.z
        );

        // Escala proporcional ao ecrã
        float scaleFactor = camHeight / 10f;
        transform.localScale = Vector3.one * scaleFactor;
    }
}
