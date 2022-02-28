using UnityEngine;

public class Timer
{
	#region Properties
	public float ElapsedTime;
	public bool IsStopped { get; private set; }
    #endregion

    #region Constructor
    public Timer()
	{
		IsStopped = false;
		ElapsedTime = 0.0f;
	}
    #endregion

    #region Timer controls
    public void Play() => IsStopped = false;

    public void Pause() => IsStopped = true;

    public void Reset() => ElapsedTime = 0.0f;

    public void Stop()
	{
		IsStopped = true;
		ElapsedTime = 0.0f;
	}

	public void Restart()
	{
		IsStopped = false;
		ElapsedTime = 0.0f;
	}
	#endregion
}