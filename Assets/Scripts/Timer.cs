using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    private TMP_Text timerText;
    enum TimerType {Countdown, Stopwatch}
    private TimerType timerType;
    private float timeToDisplay = 0.0f;

    private bool isRunning;
    
    private void Awake()
    {
        timerText = GetComponent<TMP_Text>();

        if(SceneManager.GetActiveScene().name == "Practice") {
            timerType = TimerType.Stopwatch;
            timeToDisplay = 0.0f;
        }
        else {
            timerType = TimerType.Countdown;
            timeToDisplay = 70.0f;
        }
    }

    private void OnEnable()
    {
        EventManager.TimerStart += EventManageronTimerStart;
        EventManager.TimerStop += EventManageronTimerStop;
        EventManager.TimerUpdate += EventManageronTimerUpdate;
    }

    private void OnDisable()
    {
        EventManager.TimerStart -= EventManageronTimerStart;
        EventManager.TimerStop -= EventManageronTimerStop;
        EventManager.TimerUpdate -= EventManageronTimerUpdate;
    }

    private void EventManageronTimerStart() => isRunning = true;
    private void EventManageronTimerStop() => isRunning = false;

    private void EventManageronTimerUpdate(float _value) => timeToDisplay += _value;

    private void Update(){
        if (!isRunning) return;
        if (timerType == TimerType.Countdown && timeToDisplay < 0.0f) {
            MenuController._MENUCONTROLLER.LostScene();
        }

        timeToDisplay += timerType == TimerType.Countdown ? -Time.deltaTime : Time.deltaTime;

        TimeSpan timeSpan = TimeSpan.FromSeconds(timeToDisplay);
        timerText.text = timeSpan.ToString(@"mm\:ss\:ff");
    }
}
