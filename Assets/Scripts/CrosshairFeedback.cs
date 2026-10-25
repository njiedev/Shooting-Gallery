using UnityEngine;
using UnityEngine.UI;

// Crosshair reactions: a pulse when firing, a hit marker on a hit, and a ring showing the gun cooldown.
public class CrosshairFeedback : MonoBehaviour
{
    public RawImage crosshair;
    public RawImage hitMarker;
    public Image cooldownRing;

    const float HitMarkerTime = 0.25f;

    float hitMarkerAge = HitMarkerTime;

    void Update()
    {
        crosshair.rectTransform.localScale = Vector3.Lerp(crosshair.rectTransform.localScale, Vector3.one, Time.deltaTime * 12f);

        hitMarkerAge += Time.deltaTime;
        float t = Mathf.Clamp01(hitMarkerAge / HitMarkerTime);
        hitMarker.enabled = t < 1f;
        hitMarker.rectTransform.localScale = Vector3.one * Mathf.Lerp(1.3f, 1f, t);
        hitMarker.color = new Color(1f, 0.25f, 0.2f, 1f - t * t);
    }

    public void Pulse()
    {
        crosshair.rectTransform.localScale = Vector3.one * 1.6f;
    }

    public void ShowHitMarker()
    {
        hitMarkerAge = 0f;
    }

    // fraction is the share of the cooldown still remaining (1 just after firing, 0 when ready).
    public void SetCooldown(float fraction)
    {
        cooldownRing.fillAmount = fraction;
        cooldownRing.enabled = fraction > 0f;
    }
}
