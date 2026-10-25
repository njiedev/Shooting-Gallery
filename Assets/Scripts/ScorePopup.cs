using UnityEngine;
using UnityEngine.UI;

// Floating "+10" text that rises from the hit point and fades out.
public class ScorePopup : MonoBehaviour
{
    const float Lifetime = 0.9f;
    const float Rise = 80f;

    Vector3 worldPosition;
    Camera viewCamera;
    RectTransform layer;
    RectTransform rect;
    Text text;
    Color color;
    float age;

    public static void Show(RectTransform layer, Font font, string label, Color color, Vector3 worldPosition, Camera viewCamera)
    {
        GameObject popupObject = new GameObject("ScorePopup", typeof(RectTransform));
        popupObject.transform.SetParent(layer, false);

        Text text = popupObject.AddComponent<Text>();
        text.font = font;
        text.fontSize = 44;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        text.text = label;

        Outline outline = popupObject.AddComponent<Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.8f);
        outline.effectDistance = new Vector2(2, -2);

        ScorePopup popup = popupObject.AddComponent<ScorePopup>();
        popup.worldPosition = worldPosition;
        popup.viewCamera = viewCamera;
        popup.layer = layer;
        popup.rect = popupObject.GetComponent<RectTransform>();
        popup.text = text;
        popup.color = color;
        popup.Place();
    }

    void Update()
    {
        age += Time.unscaledDeltaTime;
        if (age >= Lifetime)
        {
            Destroy(gameObject);
            return;
        }

        Place();
    }

    void Place()
    {
        float t = age / Lifetime;

        Vector3 screenPoint = viewCamera.WorldToScreenPoint(worldPosition);
        text.enabled = screenPoint.z > 0;

        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(layer, screenPoint, null, out local);
        rect.anchoredPosition = local + new Vector2(0, Rise * t);
        rect.localScale = Vector3.one * (t < 0.15f ? Mathf.Lerp(0.6f, 1.15f, t / 0.15f) : Mathf.Lerp(1.15f, 1f, (t - 0.15f) / 0.85f));

        Color faded = color;
        faded.a = t < 0.6f ? 1f : 1f - (t - 0.6f) / 0.4f;
        text.color = faded;
    }
}
