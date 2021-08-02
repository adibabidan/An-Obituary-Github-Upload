using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    private List<Tuple<float, Action>> timerList = new List<Tuple<float, Action>>() {};

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetTimer(float timerLength, Action callback)
    {
        timerList.Add(new Tuple<float, Action>(Time.realtimeSinceStartup + timerLength, callback));
    }

    void Update()
    {
        for (int i = timerList.Count; i > 0; i--) {
            var listItem = timerList[i-1];
            if (listItem.Item1 <= Time.realtimeSinceStartup) {
                listItem.Item2.Invoke();
                timerList.Remove(listItem);
            }
        }
    }
}