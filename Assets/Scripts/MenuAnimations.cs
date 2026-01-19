using UnityEngine;

public class MenuAnimations : MonoBehaviour
{

    public float amplitude = 0.2f;
    public float speed = 1.5f;

    public Vector3 initialPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float newpos = Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = initialPosition + new Vector3(0,newpos,0);
    }
}
