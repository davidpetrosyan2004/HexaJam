using UnityEngine;

public class ShoreWaveAnim : MonoBehaviour
{
    public float speed = 1.2f;
    public float scaleAmount = 0.035f;

    Vector3 startScale;
    Material mat;

    void Start()
    {
        startScale = transform.localScale;
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;

        transform.localScale = startScale * (1f + t * scaleAmount);

        Color c = mat.color;
        c.a = Mathf.Lerp(0.14f, 0.02f, t);
        mat.color = c;
    }
}