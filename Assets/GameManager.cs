using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UIManager ui;
    public int lives = 3;
    public float timeLimit = 60f;

    int score, collected, totalCoins, best;
    float timeLeft;
    bool over;
    AudioClip hurt;

    void Awake() { Instance = this; Time.timeScale = 1f; }

    void Start()
    {
        totalCoins = FindObjectsByType<Coin>(FindObjectsSortMode.None).Length;
        best = PlayerPrefs.GetInt("best", 0);
        timeLeft = timeLimit;
        ui.Init(lives, timeLimit, best);
        ui.SetScore(0, false);
        ui.SetCoins(0, totalCoins);
    }

    void Update()
    {
        if (Keyboard.current.kKey.wasPressedThisFrame) LoseLife(); // TEMP test, delete later

        if (over)
        {
            if (Keyboard.current.rKey.wasPressedThisFrame) Restart();
            return;
        }
        timeLeft -= Time.deltaTime;
        ui.SetTime(timeLeft);
        if (timeLeft <= 0) End(false);
    }

    public void Restart() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void AddScore(int amount) { score += amount; ui.SetScore(score, true); }

    public void AddCoin(int value)
    {
        score += value; collected++;
        ui.SetScore(score, true);
        ui.SetCoins(collected, totalCoins);
        if (collected >= totalCoins) End(true);
    }

    public void LoseLife()
    {
        if (over) return;
        lives--;
        ui.SetLives(lives);
        ui.Flash();
        PlayHurt();
        var cam = Camera.main.GetComponent<CameraFollow>();
        if (cam) cam.Shake(0.6f);
        if (lives <= 0) End(false);
    }

    void PlayHurt()
    {
        if (hurt == null)
        {
            int rate = 44100, n = (int)(rate * 0.3f);
            float[] d = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)rate;
                float f = 300f - 200f * (i / (float)n); // pitch drops
                d[i] = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * f * t)) * (1f - i / (float)n) * 0.25f;
            }
            hurt = AudioClip.Create("hurt", n, 1, rate, false);
            hurt.SetData(d, 0);
        }
        AudioSource.PlayClipAtPoint(hurt, Camera.main.transform.position);
    }

    void End(bool win)
    {
        if (over) return;
        over = true;
        int bonus = win ? Mathf.CeilToInt(timeLeft) * 10 : 0;
        score += bonus;
        bool newBest = score > best;
        if (newBest) { best = score; PlayerPrefs.SetInt("best", best); PlayerPrefs.Save(); }
        ui.ShowEnd(win, score, best, newBest, bonus);
        Time.timeScale = 0f;
    }
}