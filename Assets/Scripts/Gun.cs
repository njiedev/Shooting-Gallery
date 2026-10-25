using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public float range = 100f;
    public float cooldown = 1f;

    public ParticleSystem muzzleFlash;

    public AudioClip fireSound;
    public AudioClip hitMarkerSound;
    public CrosshairFeedback crosshair;
    public GunRecoil recoil;

    private Camera fpsCamera;
    private float nextTimeToFire;
    private AudioSource gunAudio;

    void Start()
    {
        fpsCamera = GameObject.Find("FirstPersonCharacter").GetComponent<Camera>();
        nextTimeToFire = 0.0f;
        gunAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        bool ready = Time.time >= nextTimeToFire;

        // Input.GetButtonDown("Fire1") is unavailable because the project uses the Input System only.
        // Fire1 defaulted to the left mouse button and left Ctrl.
        bool fire = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                    || (Keyboard.current != null && Keyboard.current.leftCtrlKey.wasPressedThisFrame);

        bool allowed = GameManager.Instance == null || GameManager.Instance.CanShoot;

        if (ready && fire && allowed)
        {
            Shoot();
        }

        crosshair.SetCooldown(Mathf.Clamp01((nextTimeToFire - Time.time) / cooldown));
    }

    void Shoot()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        gunAudio.PlayOneShot(fireSound, 0.5f);
        recoil.Kick();
        crosshair.Pulse();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterShot();
        }

        // Cooldown applies to every shot, so missing can't be spammed.
        nextTimeToFire = Time.time + cooldown; // cooldown in seconds

        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            Debug.Log("hit: " + hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null) // hit an actual target
            {
                target.Process(hit);

                if (target.points > 0)
                {
                    crosshair.ShowHitMarker();
                    gunAudio.PlayOneShot(hitMarkerSound, 0.6f);
                }

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.RegisterHit(target, hit);
                }
            }
        }
    }
}
