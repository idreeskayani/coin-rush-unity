using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 10;
    public float bobHeight = 0.15f, bobSpeed = 3f;
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    static AudioClip generated;
    Vector3 start;

    void Start() { start = transform.position; }

    void Update()
    {
        transform.position = start + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobHeight;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null) return;

        AudioClip clip = pickupSound != null ? pickupSound : GetBeep();
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);

        if (pickupEffect) Instantiate(pickupEffect, transform.position, Quaternion.identity);

        GameManager.Instance.AddCoin(value);
        Destroy(gameObject);
    }

    // Builds a short two-tone "coin" beep in code, so no audio file is needed
    static AudioClip GetBeep()
    {
        if (generated != null) return generated;
        int rate = 44100, n = (int)(rate * 0.18f);
        float[] data = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)rate;
            float freq = t < 0.07f ? 988f : 1319f;
            data[i] = Mathf.Sin(2 * Mathf.PI * freq * t) * (1f - i / (float)n) * 0.3f;
        }
        generated = AudioClip.Create("coinBeep", n, 1, rate, false);
        generated.SetData(data, 0);
        return generated;
    }
}