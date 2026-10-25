using UnityEngine;

// Puts a knocked-over target stack back together a few seconds after it is disturbed.
public class StackReset : MonoBehaviour
{
    public float resetDelay = 4f;

    Rigidbody[] cubes;
    Vector3[] startPositions;
    Quaternion[] startRotations;
    float disturbedSince = -1f;

    void Start()
    {
        cubes = GetComponentsInChildren<Rigidbody>();
        startPositions = new Vector3[cubes.Length];
        startRotations = new Quaternion[cubes.Length];

        for (int i = 0; i < cubes.Length; i++)
        {
            startPositions[i] = cubes[i].transform.localPosition;
            startRotations[i] = cubes[i].transform.localRotation;
        }
    }

    void Update()
    {
        if (disturbedSince < 0f)
        {
            if (IsDisturbed())
            {
                disturbedSince = Time.time;
            }
        }
        else if (Time.time - disturbedSince >= resetDelay)
        {
            ResetStack();
        }
    }

    bool IsDisturbed()
    {
        for (int i = 0; i < cubes.Length; i++)
        {
            if (Vector3.Distance(cubes[i].transform.localPosition, startPositions[i]) > 0.05f
                || Quaternion.Angle(cubes[i].transform.localRotation, startRotations[i]) > 2f)
            {
                return true;
            }
        }

        return false;
    }

    void ResetStack()
    {
        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].linearVelocity = Vector3.zero;
            cubes[i].angularVelocity = Vector3.zero;
            cubes[i].transform.localPosition = startPositions[i];
            cubes[i].transform.localRotation = startRotations[i];
            cubes[i].position = cubes[i].transform.position;
            cubes[i].rotation = cubes[i].transform.rotation;
        }

        disturbedSince = -1f;
        if (GetComponent<SpawnPop>() == null)
        {
            gameObject.AddComponent<SpawnPop>();
        }
    }
}
