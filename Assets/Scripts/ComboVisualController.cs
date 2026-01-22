using System.Collections;
using UnityEngine;

public class ComboVisualController : MonoBehaviour
{
    [Header("UI")]
    public RectTransform comboRect;

    [Header("Sprites")]
    public UnityEngine.UI.Image comboImage;

    public Sprite comboSprite1;
    public Sprite comboSprite2;
    public Sprite comboSprite3;

    [Header("Combo Logic")]
    public float comboWindowTime = 4f;

    public int comboChainCount = 0;
    public float comboWindowStartTime = -1f;



    [Header("Particles")]
    public ParticleSystem comboParticles;


    [Header("Movement")]
    public float speed = 2000f;
    public float middleY = 0f;
    public float offscreenY = 1000f;
    public float pauseInMiddle = 0.3f;

    [Header("Scale Effect")]
    public float scaleUp = 1.2f;       // quanto aumenta
    public float scaleSpeed = 10f;     // velocidade do zoom

    public void ShowCombo(int combo)
    {
        float currentTime = Time.time;

        // Se não há combo ativo, inicia a janela
        if (comboChainCount == 0)
        {
            comboWindowStartTime = currentTime;
            comboChainCount = 1;
        }
        else
        {
            // Se ainda está dentro da janela dos 4 segundos
            if (currentTime - comboWindowStartTime <= comboWindowTime)
            {
                comboChainCount++;
            }
            else
            {
                // Janela expirou → reset
                comboWindowStartTime = currentTime;
                comboChainCount = 1;
            }
        }

        // Escolher sprite consoante a cadeia
        if (comboChainCount == 1)
            comboImage.sprite = comboSprite1;
        else if (comboChainCount == 2)
            comboImage.sprite = comboSprite2;
        else
            comboImage.sprite = comboSprite3;

        gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(Animate());
    }



    IEnumerator Animate()
    {
        comboRect.gameObject.SetActive(true);

        // ✅ Ativar partículas
        if (comboParticles != null)
            comboParticles.Play();

        // reset
        comboRect.localScale = Vector3.one;
        comboRect.anchoredPosition = new Vector2(0, offscreenY);

        // ↓ descer até ao meio
        while (comboRect.anchoredPosition.y > middleY)
        {
            comboRect.anchoredPosition += Vector2.down * speed * Time.deltaTime;
            yield return null;
        }

        // 🔍 ZOOM IN
        yield return StartCoroutine(ScaleTo(Vector3.one * scaleUp));

        yield return new WaitForSeconds(pauseInMiddle);

        // 🔙 ZOOM OUT
        yield return StartCoroutine(ScaleTo(Vector3.one));

        // ↓ sair por baixo
        while (comboRect.anchoredPosition.y > -offscreenY)
        {
            comboRect.anchoredPosition += Vector2.down * speed * Time.deltaTime;
            yield return null;
        }

        comboRect.gameObject.SetActive(false);

        // ✅ Parar partículas
        if (comboParticles != null)
            comboParticles.Stop();
    }


    IEnumerator ScaleTo(Vector3 targetScale)
    {
        while (Vector3.Distance(comboRect.localScale, targetScale) > 0.01f)
        {
            comboRect.localScale = Vector3.Lerp(
                comboRect.localScale,
                targetScale,
                Time.deltaTime * scaleSpeed
            );
            yield return null;
        }

        comboRect.localScale = targetScale;
    }
}
