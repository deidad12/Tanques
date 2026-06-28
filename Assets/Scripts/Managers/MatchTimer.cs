using UnityEngine;

public class MatchTimer : MonoBehaviour
{
    public static MatchTimer Instance;

    public float maxTime = 240f; // 4 minutos = 240 segundos

    private float startTime;
    private bool running = true;

    private void Awake()
    {
        // ✅ Evita múltiples timers (bug MUY común)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        startTime = Time.time;
    }

    public float GetRemainingTime()
    {
        if (!running) return 0f;

        float elapsed = Time.time - startTime;
        float remaining = maxTime - elapsed;

        return Mathf.Max(remaining, 0f);
    }

    public bool IsTimeOver()
    {
        return GetRemainingTime() <= 0f;
    }

    public void StopTimer()
    {
        running = false;
    }
}