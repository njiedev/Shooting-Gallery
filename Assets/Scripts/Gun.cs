using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public float range = 100f;

    public ParticleSystem muzzleFlash;

    private Camera fpsCamera;
    private float nextTimeToFire;

    void Start()
    {
        fpsCamera = GameObject.Find("FirstPersonCharacter").GetComponent<Camera>();
        nextTimeToFire = 0.0f;
    }

    void Update()
    {
        bool ready = Time.time >= nextTimeToFire;

        // Input.GetButtonDown("Fire1") is unavailable because the project uses the Input System only.
        // Fire1 defaulted to the left mouse button and left Ctrl.
        bool fire = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                    || (Keyboard.current != null && Keyboard.current.leftCtrlKey.wasPressedThisFrame);

        if (ready && fire)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            Debug.Log("hit: " + hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null) // hit an actual target
            {
                target.Process(hit);
            }

            nextTimeToFire = Time.time + 1f; // cooldown in seconds
        }
    }
}
