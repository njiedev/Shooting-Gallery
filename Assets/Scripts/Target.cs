using UnityEngine;

public class Target : MonoBehaviour
{
    public GameObject target;

    public GameObject effectsManager;
    public GameObject hitEffect;
    public float effectDuration = 0.1f;

    public AudioClip hitSound;

    // Score awarded by the GameManager when this part of the target is shot (negative for penalties).
    public int points;

    protected Effect effectScript;

    private void Awake()
    {
        // initialization must occur in Awake method
        effectScript = effectsManager.GetComponent<Effect>();
    }

    public virtual void Process(RaycastHit hit)
    {
    }
}
