using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Counts down from a certian point in time, updating its text accordingly.
/// </summary>
public class CountdownTimer : MonoBehaviour
{
    public float demoTime = 300;
    int minutes;
    int seconds;

    bool timerRunning = false;

    TMP_Text text;

    public UnityEvent timerExpired;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        StartCoroutine(waitASec());

        text.text = $"Remaining Time: {minutes}:{seconds:00}";
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            demoTime -= Time.unscaledDeltaTime;

            minutes = Mathf.CeilToInt(demoTime) / 60;
            seconds = Mathf.CeilToInt(demoTime) % 60;

            text.text = $"Remaining Time: {minutes}:{seconds:00}";

            if (demoTime < 0)
            {
                OnTimerExpired();
            }

        }
    }

    //If you don't wait a second after start to turn on the timer then the timer starts before you finish loading in
    IEnumerator waitASec()
    {
        yield return new WaitForSeconds(1);
        timerRunning = true;
    }

    void OnTimerExpired()
    {
        timerRunning = false;

        timerExpired.Invoke();
    }
}
