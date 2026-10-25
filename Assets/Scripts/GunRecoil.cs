using UnityEngine;

// Kicks the gun back and up when it fires, then springs it back to its resting pose.
public class GunRecoil : MonoBehaviour
{
    public float kickBack = 0.08f;
    public float kickUp = 8f;
    public float returnSpeed = 10f;

    Vector3 restPosition;
    Quaternion restRotation;
    float kick;

    void Start()
    {
        restPosition = transform.localPosition;
        restRotation = transform.localRotation;
    }

    void Update()
    {
        kick = Mathf.Lerp(kick, 0f, Time.deltaTime * returnSpeed);
        transform.localPosition = restPosition + new Vector3(0, 0, -kickBack * kick);
        transform.localRotation = restRotation * Quaternion.Euler(-kickUp * kick, 0, 0);
    }

    public void Kick()
    {
        kick = 1f;
    }
}
