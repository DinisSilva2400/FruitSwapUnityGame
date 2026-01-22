using UnityEngine;

public class ExplosionAutoDestroy : MonoBehaviour
{
    void Start()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            float clipLength = anim.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, clipLength); // destrói automaticamente após a animação
        }
        else
        {
            Destroy(gameObject, 1f); // fallback
        }
    }
}
