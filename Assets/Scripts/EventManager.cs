using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager {
    public static event UnityAction TimerStart;
    public static event UnityAction TimerStop;
    public static event UnityAction<float> TimerUpdate;

    public static void onTimerStart() => TimerStart?.Invoke();
    public static void onTimerStop() => TimerStop?.Invoke();
    public static void onTimerUpdate(float _value) => TimerUpdate?.Invoke(_value);
}

