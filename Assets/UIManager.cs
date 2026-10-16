using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    public TMP_Text scoreText, bestText, coinsText, timeText;
    public Image[] hearts;
    public Slider timeBar;
    public Image damageFlash;

    [Header("End Screen")]
    public CanvasGroup endPanel;
    public TMP_Text titleText, summaryText;

    public Color heartOn = new Color(1f, 0.3f, 0.35f);
    public Color heartOff = new Color(0.25f, 0.25f, 0.25f, 0.6f);

    float timeLimit;
    Image barFill;
    Coroutine punch;

    public void Init(int lives, float limit, int best)
    {
        timeLimit = limit;
        barFill = timeBar.fillRect.GetComponent<Image>();
        timeBar.minValue = 0; timeBar.maxValue = limit; timeBar.value = limit;
        bestText.text = "BEST " + best;
        damageFlash.color = new Color(1, 0, 0, 0);
        endPanel.alpha = 0;
        endPanel.gameObject.SetActive(false);
        SetLives(lives);
    }

    public void SetScore(int score, bool animate)
    {
        scoreText.text = score.ToString("D5");
        if (!animate) return;
        if (punch != null) StopCoroutine(punch);
        punch = StartCoroutine(Punch(scoreText.transform));
    }

    public void SetCoins(int c, int total) { coinsText.text = c + " / " + total; }

    public void SetLives(int lives)
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].color = i < lives ? heartOn : heartOff;
    }

    public void SetTime(float t)
    {
        t = Mathf.Max(0, t);
        timeBar.value = t;
        timeText.text = Mathf.CeilToInt(t).ToString();
        timeText.color = t < 10 ? Color.red : Color.white;
        barFill.color = Color.Lerp(new Color(1f, 0.25f, 0.2f), new Color(0.3f, 0.9f, 0.4f), t / timeLimit);
    }

    public void Flash() { StartCoroutine(FlashRoutine()); }

    public void ShowEnd(bool win, int score, int best, bool newBest, int bonus)
    {
        titleText.text = win ? "YOU WIN!" : "GAME OVER";
        titleText.color = win ? new Color(0.4f, 1f, 0.5f) : new Color(1f, 0.35f, 0.3f);
        summaryText.text = "Score  " + score
            + (bonus > 0 ? "   (time bonus +" + bonus + ")" : "")
            + "\nBest  " + best + (newBest ? "   NEW RECORD!" : "");
        endPanel.gameObject.SetActive(true);
        StartCoroutine(Fade());
    }

    IEnumerator Punch(Transform t)
    {
        float d = 0.2f;
        for (float e = 0; e < d; e += Time.unscaledDeltaTime)
        {
            t.localScale = Vector3.one * (1f + 0.35f * Mathf.Sin(e / d * Mathf.PI));
            yield return null;
        }
        t.localScale = Vector3.one;
    }

    IEnumerator FlashRoutine()
    {
        float d = 0.4f;
        for (float e = 0; e < d; e += Time.unscaledDeltaTime)
        {
            damageFlash.color = new Color(1, 0, 0, 0.4f * (1 - e / d));
            yield return null;
        }
        damageFlash.color = new Color(1, 0, 0, 0);
    }

    IEnumerator Fade()
    {
        float d = 0.5f;
        for (float e = 0; e < d; e += Time.unscaledDeltaTime)
        {
            endPanel.alpha = e / d;
            yield return null;
        }
        endPanel.alpha = 1;
    }
}