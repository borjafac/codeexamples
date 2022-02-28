using System.Collections.Generic;
using UnityEngine;

public class TimerFactory : MonoBehaviour
{
    private List<Timer> TimersList;

    private void Awake() => TimersList = new List<Timer>();

    private void Update() => UpdateTimers();

    private void UpdateTimers()
    {
        float deltaTime = Time.deltaTime;

        foreach(Timer timer in TimersList)
        {
            if(!timer.IsStopped)
                timer.ElapsedTime += deltaTime;
        }	
    }

    public Timer CreateTimer()
    {
        Timer newTimer = new Timer();
		TimersList.Add(newTimer);
        return TimersList[TimersList.Count - 1];
    }
}