using UnityEngine;

// Shakes the camera by offsetting its local position; rotation is left to the first-person controller.
public class CameraShake : MonoBehaviour
{
    Vector3 restPosition;
    float duration;
    float magnitude;
    float remaining;

    void Start()
    {
        restPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        if (remaining <= 0f)
        {
            transform.localPosition = restPosition;
            return;
        }

        remaining -= Time.deltaTime;
        float strength = magnitude * Mathf.Clamp01(remaining / duration);
        transform.localPosition = restPosition + Random.insideUnitSphere * strength;
    }

    public void Shake(float shakeDuration, float shakeMagnitude)
    {
        duration = shakeDuration;
        magnitude = shakeMagnitude;
        remaining = shakeDuration;
    }
}
