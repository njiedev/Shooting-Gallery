using UnityEngine;

// Scales an object up from nothing with a little overshoot, then removes itself.
public class SpawnPop : MonoBehaviour
{
    const float Duration = 0.35f;

    Vector3 targetScale;
    float age;

    void Awake()
    {
        targetScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        age += Time.deltaTime;
        float t = Mathf.Clamp01(age / Duration);

        // Ease-out-back: rises past full size and settles back.
        float s = 1f + 2.7f * Mathf.Pow(t - 1f, 3) + 1.7f * Mathf.Pow(t - 1f, 2);
        transform.localScale = targetScale * s;

        if (t >= 1f)
        {
            transform.localScale = targetScale;
            Destroy(this);
        }
    }
}
