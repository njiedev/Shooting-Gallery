using UnityEngine;

// Slides an object back and forth along an axis, like a carnival target on a track.
public class Mover : MonoBehaviour
{
    public Vector3 axis = Vector3.right;
    public float amplitude = 3f;
    public float speed = 1f;
    public float phase;

    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.position = startPosition + axis.normalized * (Mathf.Sin(Time.time * speed + phase) * amplitude);
    }
}
