using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ScaleToScreen : MonoBehaviour
{
    void Start()
    {
        Camera cam = Camera.main;
        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Vector2 spriteSize = sr.sprite.bounds.size;

        float scaleX = width / spriteSize.x;
        float scaleY = height / spriteSize.y;

        // Estica em ambos os eixos para preencher o ecrã todo
        transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}