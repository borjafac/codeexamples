using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatesInfo : MonoBehaviour
{
	#region Properties
	public State<Hydration> DogHydrationState { get; private set; }
	public State<Feeding> DogFeedingState { get; private set; }
	public State<Sleep> DogSleepState { get; private set; }
	public State<Fun> DogFunState { get; private set; }
	public State<Hygiene> DogHygieneState { get; private set; }
	public State<Happiness> DogHappinessState { get; private set; }
	public State<ExcrementLoad> DogPooLoadState { get; private set; }
	public State<ExcrementLoad> DogPeeLoadState { get; private set; }

	public int[] HydrationThresholds = new int[2];
	public int[] FeedingThresholds = new int[2];
	public int[] SleepThresholds = new int[2];
	public int[] FunThresholds = new int[2];
	public int[] HygieneThresholds = new int[2];
	public int[] HappinessThresholds = new int[2];
	public int[] PooThresholds = new int[2];
	public int[] PeeThresholds = new int[2];
	#endregion

	#region Unity Functions
	private void Awake()
	{
		DogHydrationState = new State<Hydration>(HydrationThresholds[0], HydrationThresholds[1]);
		DogFeedingState = new State<Feeding>(FeedingThresholds[0], FeedingThresholds[1]);
		DogSleepState = new State<Sleep>(SleepThresholds[0], SleepThresholds[1]);
		DogFunState = new State<Fun>(FunThresholds[0], FunThresholds[1]);
		DogHygieneState = new State<Hygiene>(HygieneThresholds[0], HygieneThresholds[1]);
		DogHappinessState = new State<Happiness>(HappinessThresholds[0], HappinessThresholds[1]);
		DogPooLoadState = new State<ExcrementLoad>(PooThresholds[0], PooThresholds[1]);
		DogPeeLoadState = new State<ExcrementLoad>(PeeThresholds[0], PeeThresholds[1]);

		DogHydrationState.ActualState = Hydration.Hydrated;
		DogFeedingState.ActualState = Feeding.Fed;
		DogSleepState.ActualState = Sleep.Awake;
		DogFunState.ActualState = Fun.Cheerful;
		DogHygieneState.ActualState = Hygiene.Shiny;
		DogHappinessState.ActualState = Happiness.Happy;
		DogPooLoadState.ActualState = ExcrementLoad.LOAD_0;
		DogPeeLoadState.ActualState = ExcrementLoad.LOAD_0;
	}

	private void OnEnable()
	{
		DogHydrationState.OnIndicatorChange += OnHydrationIndicatorChange;
		DogFeedingState.OnIndicatorChange += OnFeedingIndicatorChange;
		DogSleepState.OnIndicatorChange += OnSleepIndicatorChange;
		DogFunState.OnIndicatorChange += OnFunIndicatorChange;
		DogHygieneState.OnIndicatorChange += OnHygieneIndicatorChange;
		DogHappinessState.OnIndicatorChange += OnHappinessIndicatorChange;
	}

	private void OnDisable()
	{
		DogHydrationState.OnIndicatorChange -= OnHydrationIndicatorChange;
		DogFeedingState.OnIndicatorChange -= OnFeedingIndicatorChange;
		DogSleepState.OnIndicatorChange -= OnSleepIndicatorChange;
		DogFunState.OnIndicatorChange -= OnFunIndicatorChange;
		DogHygieneState.OnIndicatorChange -= OnHygieneIndicatorChange;
		DogHappinessState.OnIndicatorChange -= OnHappinessIndicatorChange;
	}
	#endregion

	#region Indicator listeners
	private void OnHydrationIndicatorChange()
	{
		if(DogHydrationState.Indicator >= DogHydrationState.Thresholds[DogHydrationState.Thresholds.Length - 1] && DogHydrationState.ActualState != Hydration.Hydrated)
		{
			DogHydrationState.ActualState = Hydration.Hydrated;
			return;
		}

		for(int i = 0; i < DogHydrationState.Thresholds.Length; i++)
		{
			if(DogHydrationState.Indicator < DogHydrationState.Thresholds[i])
			{
				if(DogHydrationState.ActualState != (Hydration)i)
					DogHydrationState.ActualState = (Hydration)i;

				break;
			}
		}
	}

	private void OnFeedingIndicatorChange()
	{
		if(DogFeedingState.Indicator >= DogFeedingState.Thresholds[DogFeedingState.Thresholds.Length - 1] && DogFeedingState.ActualState != Feeding.Fed)
		{
			DogFeedingState.ActualState = Feeding.Fed;
			return;
		}

		for(int i = 0; i < DogFeedingState.Thresholds.Length; i++)
		{
			if(DogFeedingState.Indicator < DogFeedingState.Thresholds[i])
			{
				if(DogFeedingState.ActualState != (Feeding)i)
					DogFeedingState.ActualState = (Feeding)i;

				break;
			}
		}
	}

	private void OnSleepIndicatorChange()
	{
		if(DogSleepState.Indicator >= DogSleepState.Thresholds[DogSleepState.Thresholds.Length - 1] && DogSleepState.ActualState != Sleep.Awake)
		{
			DogSleepState.ActualState = Sleep.Awake;
			return;
		}

		for(int i = 0; i < DogSleepState.Thresholds.Length; i++)
		{
			if(DogSleepState.Indicator < DogSleepState.Thresholds[i])
			{
				if(DogSleepState.ActualState != (Sleep)i)
					DogSleepState.ActualState = (Sleep)i;

				break;
			}
		}
	}

	private void OnFunIndicatorChange()
	{
		if(DogFunState.Indicator >= DogFunState.Thresholds[DogFunState.Thresholds.Length - 1] && DogFunState.ActualState != Fun.Cheerful)
		{
			DogFunState.ActualState = Fun.Cheerful;
			return;
		}

		for(int i = 0; i < DogFunState.Thresholds.Length; i++)
		{
			if(DogFunState.Indicator < DogFunState.Thresholds[i])
			{
				if(DogFunState.ActualState != (Fun)i)
					DogFunState.ActualState = (Fun)i;

				break;
			}
		}
	}

	private void OnHygieneIndicatorChange()
	{
		if(DogHygieneState.Indicator >= DogHygieneState.Thresholds[DogHygieneState.Thresholds.Length - 1] && DogHygieneState.ActualState != Hygiene.Shiny)
		{
			DogHygieneState.ActualState = Hygiene.Shiny;
			return;
		}

		for(int i = 0; i < DogHygieneState.Thresholds.Length; i++)
		{
			if(DogHygieneState.Indicator < DogHygieneState.Thresholds[i])
			{
				if(DogHygieneState.ActualState != (Hygiene)i)
				{
					DogHygieneState.ActualState = (Hygiene)i;
					break;
				}

				break;
			}
		}
	}

	private void OnHappinessIndicatorChange()
	{
		if(DogHappinessState.Indicator >= DogHappinessState.Thresholds[DogHygieneState.Thresholds.Length - 1] && DogHappinessState.ActualState != Happiness.Happy)
		{
			DogHappinessState.ActualState = Happiness.Happy;
			return;
		}

		for(int i = 0; i < DogHappinessState.Thresholds.Length; i++)
		{
			if(DogHappinessState.Indicator < DogHappinessState.Thresholds[i])
			{
				if(DogHappinessState.ActualState != (Happiness)i)
				{
					DogHappinessState.ActualState = (Happiness)i;
					break;
				}

				break;
			}
		}

		if(DogHappinessState.Indicator <= 0)
			GM.Inst.endGameController.EndGame();
	}
    #endregion

    #region Testing Functions
    public void AddHydration() => DogHydrationState.Indicator++;
    public void SubsHydration() => DogHydrationState.Indicator--;
    public void AddFeeding() => DogFeedingState.Indicator++;
    public void SubsFeeding() => DogFeedingState.Indicator--;
    public void AddSleep() => DogFeedingState.Indicator++;
    public void SubsSleep() => DogSleepState.Indicator--;
    public void AddFun() => DogFeedingState.Indicator++;
    public void SubsFun() => DogFeedingState.Indicator--;
    public void AddHygiene() => DogFeedingState.Indicator++;
    public void SubsHygiene() => DogFeedingState.Indicator--;
    public void AddHappiness() => DogFeedingState.Indicator++;
    public void SubsHappiness() => DogFeedingState.Indicator--;
    #endregion
}
