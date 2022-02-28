using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State<T>
{
	public StateTimers stateTimers = new StateTimers();
	public int[] Thresholds;
	public delegate void OnIndicatorChangeDelegate();
	public delegate void OnStateChangeDelegate(T newVal);
	public event OnIndicatorChangeDelegate OnIndicatorChange;
	public event OnStateChangeDelegate OnStateChange;

	private int indicator = 100;
	private T actualState;

	public T ActualState 
	{
		get { return actualState; }

		set
		{
			if(actualState.Equals(value))
                return;

            actualState = value;

            if(OnStateChange != null)
				OnStateChange(actualState);
		}
	}

	public int Indicator
	{
		get{return indicator;}

		set
		{
			if(value == indicator) return;

			if(value < 0)
				indicator = 0;
			else if(value >= Defines.MaxParameterIndicator)
				indicator = Defines.MaxParameterIndicator;
			else
			{
				indicator = value;

                if(OnIndicatorChange != null)
					OnIndicatorChange();
			}
		}
	}

    public State(int firstThreshold, int secondThreshold) => Thresholds = new int[2] { firstThreshold, secondThreshold };
}
