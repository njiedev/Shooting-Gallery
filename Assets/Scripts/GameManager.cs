using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Runs a timed round: start screen -> 60 seconds of shooting -> results, with pause when the cursor is released.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    enum State { Ready, Playing, Paused, Over }

    const string BestScoreKey = "ShootingGallery.BestScore";

    public float roundLength = 60f;

    // Pausing when the cursor is released only makes sense with a real pointer.
    public bool pauseWhenCursorReleased = true;

    public FirstPersonController player;
    public CameraShake cameraShake;
    public Camera fpsCamera;

    public Text scoreText;
    public Text timeText;
    public GameObject messagePanel;
    public Text messageTitle;
    public Text messageBody;
    public RectTransform popupLayer;
    public Font font;

    State state;
    int stateFrame;
    float stateTime;
    float timeLeft;
    int score;
    int shots;
    int hits;

    public bool CanShoot => state == State.Playing && Time.frameCount > stateFrame;

    void Awake()
    {
        Instance = this;

        // Disabled before its Start runs, so the cursor isn't captured until the player clicks to start.
        player.enabled = false;
    }

    void Start()
    {
        Time.timeScale = 1f;
        timeLeft = roundLength;
        SetState(State.Ready);
        ReleaseCursor();

        ShowMessage("SHOOTING GALLERY",
            "Score as many points as you can in " + roundLength.ToString("0") + " seconds.\n\n" +
            "Bullseye  +10        Moving bullseye  +20\n" +
            "Diamond  +15        Stack block  +5\n" +
            "<color=#ff6b6b>Bomb  -25</color>   don't shoot the ornaments!\n\n" +
            "WASD move   Mouse aim   Click shoot   Esc pause\n\n" +
            "<b>CLICK TO START</b>");

        UpdateHud();
    }

    void Update()
    {
        bool clicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        switch (state)
        {
            case State.Ready:
                if (clicked)
                {
                    StartRound();
                }
                break;

            case State.Playing:
                // The grace period gives the cursor capture time to take effect after starting or resuming.
                if (pauseWhenCursorReleased && Cursor.lockState != CursorLockMode.Locked && Time.unscaledTime - stateTime > 0.2f)
                {
                    Pause();
                    break;
                }

                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0)
                {
                    EndRound();
                }
                UpdateHud();
                break;

            case State.Paused:
                if (clicked)
                {
                    Resume();
                }
                break;

            case State.Over:
                // A short delay stops a shot fired as time ran out from skipping the results.
                if (clicked && Time.unscaledTime - stateTime > 1f)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                break;
        }
    }

    public void RegisterShot()
    {
        shots++;
    }

    public void RegisterHit(Target target, RaycastHit hit)
    {
        if (state != State.Playing || target.points == 0)
        {
            return;
        }

        score += target.points;
        if (target.points > 0)
        {
            hits++;
        }

        string label = target.points > 0 ? "+" + target.points : target.points.ToString();
        Color color = target.points > 0 ? new Color(1f, 0.9f, 0.3f) : new Color(1f, 0.35f, 0.3f);
        ScorePopup.Show(popupLayer, font, label, color, hit.point, fpsCamera);

        if (target is Bomb)
        {
            cameraShake.Shake(0.4f, 0.12f);
        }

        UpdateHud();
    }

    void StartRound()
    {
        SetState(State.Playing);
        HideMessage();
        player.enabled = true;
        CaptureCursor();
    }

    void Pause()
    {
        SetState(State.Paused);
        Time.timeScale = 0f;
        ShowMessage("PAUSED", "<b>CLICK TO RESUME</b>");
    }

    void Resume()
    {
        SetState(State.Playing);
        Time.timeScale = 1f;
        HideMessage();
        CaptureCursor();
    }

    void EndRound()
    {
        SetState(State.Over);
        timeLeft = 0;
        player.enabled = false;
        ReleaseCursor();

        int best = PlayerPrefs.GetInt(BestScoreKey, 0);
        bool newBest = score > best;
        if (newBest)
        {
            best = score;
            PlayerPrefs.SetInt(BestScoreKey, best);
            PlayerPrefs.Save();
        }

        int accuracy = shots > 0 ? Mathf.RoundToInt(100f * hits / shots) : 0;

        ShowMessage("TIME'S UP!",
            "Score  <b>" + score + "</b>\n" +
            "Accuracy  " + accuracy + "%  (" + hits + " / " + shots + " shots)\n" +
            (newBest ? "<color=#ffd84d><b>NEW BEST SCORE!</b></color>" : "Best  " + best) + "\n\n" +
            "<b>CLICK TO PLAY AGAIN</b>");

        UpdateHud();
    }

    void SetState(State newState)
    {
        state = newState;
        stateFrame = Time.frameCount;
        stateTime = Time.unscaledTime;
    }

    void UpdateHud()
    {
        scoreText.text = "SCORE  " + score;

        int seconds = Mathf.CeilToInt(Mathf.Max(timeLeft, 0));
        timeText.text = (seconds / 60) + ":" + (seconds % 60).ToString("00");
        timeText.color = seconds <= 10 && state == State.Playing ? new Color(1f, 0.35f, 0.3f) : Color.white;
    }

    void ShowMessage(string title, string body)
    {
        messageTitle.text = title;
        messageBody.text = body;
        messagePanel.SetActive(true);
    }

    void HideMessage()
    {
        messagePanel.SetActive(false);
    }

    static void CaptureCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    static void ReleaseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
