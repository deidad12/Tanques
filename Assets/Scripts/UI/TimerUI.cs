using UnityEngine;

public class TimerUI : MonoBehaviour
{
    public UnityEngine.UI.Text timerText;

    private void Update()
    {
        if (MatchTimer.Instance == null) return;

        float remaining = MatchTimer.Instance.GetRemainingTime();

        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);

        timerText.text = string.Format("⏱ {0:00}:{1:00}", minutes, seconds);
    }
}