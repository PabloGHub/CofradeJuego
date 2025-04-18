using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float currentTime;
    public float time => currentTime;
    private bool isPaused = true;
    private TextMeshProUGUI timerText;
    public string textPrefix = "Tiempo: ";

    private static Timer instance;
    public static Timer Instance => instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    void Start()
    {
        currentTime = 0;
        timerText = GetComponent<TextMeshProUGUI>();
        timerText.text = textPrefix + FormatTime(currentTime);
    }

    void Update()
    {
        if (!isPaused && !ControladorPPAL.V_pausado_b)
        {
            currentTime += Time.deltaTime;
            timerText.text = textPrefix + FormatTime(currentTime);
        }
    }

    public void StartTimer()
    {
        isPaused = false;
        currentTime = 0;
    }
    public void PauseTimer()
    {
        isPaused = true;
    }
    public void ResumeTimer()
    {
        isPaused = false;
    }
    public void ResetTimer()
    {
        isPaused = true;
        currentTime = 0;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
    }
}
