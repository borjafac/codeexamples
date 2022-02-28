using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates = GM.Inst.playerStatesInfo;

[RequireComponent(typeof(PlayerStatesInfo))]
public class PlayerStateMachine : MonoBehaviour
{
	#region Properties
	public int HydrationSecondsToDecrease;
	public int FeedingSecondsToDecrease;
	public int SleepSecondsToDecrease;
	public int MoodSecondsToDecrease;
	public int HygieneSecondsToDecrease;
	public int HappinessSecondsToDecrease;
	public int SecondsToLoseAttention;
	public int TimeBetweenBoredomActions;
	public int TimeToPeeAndPooWhenLoaded;
	public bool IsDoingBlockingAction;
	public bool IsSleeping;
	public Timer AttentionLossTimer;

	private float TimeLapsedAfterUpdate;
	private Queue<Actions> BlockingActionsQueue;
	private Timer BoredomTimer;
	private int BoredomActionCounter = 0;
	private FoodBowlController FoodBowl;
	private WaterBowlController WaterBowl;
	#endregion

	#region Unity Functions
	void Start()
	{
		IsSleeping = false;
		BlockingActionsQueue = new Queue<Actions>();
		AttentionLossTimer = GM.Inst.timerFactory.CreateTimer();
		AttentionLossTimer.Play();
		BoredomTimer = GM.Inst.timerFactory.CreateTimer();
		WaterBowl = FindObjectOfType<WaterBowlController>();
		FoodBowl = FindObjectOfType<FoodBowlController>();
	}

	private void OnEnable()
	{
		PlayerStates.DogHydrationState.OnStateChange += OnHydrationStateChange;
		PlayerStates.DogFeedingState.OnStateChange += OnFeedingStateChange;
		PlayerStates.DogSleepState.OnStateChange += OnSleepStateChange;
		PlayerStates.DogFunState.OnStateChange += OnFunStateChange;
		PlayerStates.DogHygieneState.OnStateChange += OnHygieneStateChange;
		PlayerStates.DogHappinessState.OnStateChange += OnHappinessStateChange;
		PlayerStates.DogPooLoadState.OnStateChange += OnPooLoadStateChange;
		PlayerStates.DogPeeLoadState.OnStateChange += OnPeeLoadStateChange;

		GM.Inst.inputController.OnBoredomDoubleTap += DoComeBack;
	}

	private void OnDisable()
	{
		PlayerStates.DogHydrationState.OnStateChange -= OnHydrationStateChange;
		PlayerStates.DogFeedingState.OnStateChange -= OnFeedingStateChange;
		PlayerStates.DogSleepState.OnStateChange -= OnSleepStateChange;
		PlayerStates.DogFunState.OnStateChange -= OnFunStateChange;
		PlayerStates.DogHygieneState.OnStateChange -= OnHygieneStateChange;
		PlayerStates.DogHappinessState.OnStateChange -= OnHappinessStateChange;
		PlayerStates.DogPooLoadState.OnStateChange -= OnPooLoadStateChange;
		PlayerStates.DogPeeLoadState.OnStateChange -= OnPeeLoadStateChange;

		GM.Inst.inputController.OnBoredomDoubleTap -= DoComeBack;
	}

	void Update()
	{
		if(GM.Inst.simMode.ActiveMode == SimModeEnum.Furniture)
			return;

		HandleStatesIndicators();
		HandleInsideStateTimers();
		PeePooBehaviourUpdate();
		HandleAttentionTimer();
		CycleBoredomOrAttention();
	}
	#endregion

	#region Timers methods
	#region Attention timer
	private void HandleAttentionTimer()
	{
		if(GM.Inst.simMode.ActiveMode == SimModeEnum.Attention && AttentionLossTimer.ElapsedTime >= SecondsToLoseAttention)
		{
			GM.Inst.simMode.ActiveMode = SimModeEnum.Boredom;
			AttentionLossTimer.Pause();
			AttentionLossTimer.Reset();
		}
		else if(AttentionLossTimer.IsStopped && GM.Inst.simMode.ActiveMode == SimModeEnum.Attention)
			AttentionLossTimer.Play();
	}
	#endregion

	#region Inside state timers
	private void HandleInsideStateTimers()
	{
		CheckHydrationHappiness();
		CheckFeedingHappiness();
		CheckSleepHappiness();
		CheckFunHappiness();
		CheckHygieneHappiness();
		CheckBondLevel();
	}

	private void CheckHydrationHappiness()
	{
		float elapsedTime = PlayerStates.DogHydrationState.stateTimers.InsideStateTimer.ElapsedTime;

		switch(PlayerStates.DogHydrationState.ActualState)
		{
			case Hydration.Thirsty:
				if(elapsedTime > 10)
				{
					PlayerStates.DogHappinessState.Indicator--;
					PlayerStates.DogHydrationState.stateTimers.InsideStateTimer.Reset();
				}
				break;
			case Hydration.Hydrated:
				if(elapsedTime > 2)
				{
					PlayerStates.DogHappinessState.Indicator++;
					PlayerStates.DogHydrationState.stateTimers.InsideStateTimer.Reset();
				}
				break;
		}
	}

	private void CheckFeedingHappiness()
	{
		float elapsedTime = PlayerStates.DogFeedingState.stateTimers.InsideStateTimer.ElapsedTime;

		switch(PlayerStates.DogFeedingState.ActualState)
		{
			case Feeding.Hungry:
				if(elapsedTime > 10)
				{
					PlayerStates.DogHappinessState.Indicator--;
					PlayerStates.DogFeedingState.stateTimers.InsideStateTimer.Reset();
				}
				break;
			case Feeding.Fed:
				if(elapsedTime > 2)
				{
					PlayerStates.DogHappinessState.Indicator++;
					PlayerStates.DogFeedingState.stateTimers.InsideStateTimer.Reset();
				}
				break;
		}
	}

	private void CheckSleepHappiness()
	{
		float elapsedTime = PlayerStates.DogSleepState.stateTimers.InsideStateTimer.ElapsedTime;

		switch(PlayerStates.DogSleepState.ActualState)
		{
			case Sleep.Asleep:
				if(elapsedTime > 5)
				{
					PlayerStates.DogHappinessState.Indicator++;
					PlayerStates.DogSleepState.stateTimers.InsideStateTimer.Reset();
				}
				break;
			case Sleep.Sleepy:
				if(elapsedTime > 3)
				{
					PlayerStates.DogHappinessState.Indicator--;
					PlayerStates.DogSleepState.stateTimers.InsideStateTimer.Reset();
				}
				break;
			case Sleep.Awake:
				if(elapsedTime > 10)
				{
					PlayerStates.DogHappinessState.Indicator--;
					PlayerStates.DogSleepState.stateTimers.InsideStateTimer.Reset();
				}
				break;
		}
	}

	private void CheckFunHappiness()
	{
		float elapsedTime = PlayerStates.DogFunState.stateTimers.InsideStateTimer.ElapsedTime;

		switch(PlayerStates.DogFunState.ActualState)
		{
			case Fun.Bored:
				if(elapsedTime > 5)
				{
					PlayerStates.DogHappinessState.Indicator--;
					PlayerStates.DogFunState.stateTimers.InsideStateTimer.Reset();
				}
				break;
			case Fun.Cheerful:
				if(elapsedTime > 10)
				{
					PlayerStates.DogHappinessState.Indicator++;
					PlayerStates.DogFunState.stateTimers.InsideStateTimer.Reset();
				}
				break;
		}
	}

	private void CheckHygieneHappiness()
	{
		float elapsedTime = PlayerStates.DogHygieneState.stateTimers.InsideStateTimer.ElapsedTime;

		switch(PlayerStates.DogHygieneState.ActualState)
		{
			case Hygiene.Dirty:
				if(elapsedTime > 10)
				{
					PlayerStates.DogHappinessState.Indicator--;
					PlayerStates.DogHygieneState.stateTimers.InsideStateTimer.Reset();
				}
				break;
			case Hygiene.Shiny:
				if(elapsedTime > 2)
				{
					PlayerStates.DogHappinessState.Indicator++;
					PlayerStates.DogHygieneState.stateTimers.InsideStateTimer.Reset();
				}
				break;
		}
	}

	private void CheckBondLevel()
	{
		float elapsedTime = PlayerStates.DogHappinessState.stateTimers.InsideStateTimer.ElapsedTime;

		switch(PlayerStates.DogHappinessState.ActualState)
		{
			case Happiness.Sad:
				if(elapsedTime > 10)
				{
					GM.Inst.bondAndLevelData.BondWithPlayer--;
					PlayerStates.DogHappinessState.stateTimers.InsideStateTimer.Reset();
				}
				break;
			case Happiness.Happy:
				if(elapsedTime > 2)
				{
					GM.Inst.bondAndLevelData.BondWithPlayer++;
					PlayerStates.DogHappinessState.stateTimers.InsideStateTimer.Reset();
				}
				break;
		}
	}
	#endregion

	#region Indicator timers
	private void HandleStatesIndicators()
	{
		TimeLapsedAfterUpdate += Time.deltaTime;

		if(TimeLapsedAfterUpdate >= 1)
		{
			UpdateStateIndicators();
			TimeLapsedAfterUpdate = 0.0f;
		}
	}

	private void UpdateStateIndicators()
	{
		CheckHydrationTimer();
		CheckFeedingTimer();
		CheckSleepTimer();
		CheckFunTimer();
		CheckHygieneTimer();
		CheckHappinessTimer();
	}

	private void CheckHydrationTimer()
	{
		if(PlayerStates.DogHydrationState.stateTimers.IndicatorTimer.ElapsedTime >= HydrationSecondsToDecrease)
		{
			PlayerStates.DogHydrationState.Indicator--;
			PlayerStates.DogHydrationState.stateTimers.IndicatorTimer.Reset();
		}
	}

	private void CheckFeedingTimer()
	{
		if(PlayerStates.DogFeedingState.stateTimers.IndicatorTimer.ElapsedTime >= FeedingSecondsToDecrease)
		{
			PlayerStates.DogFeedingState.Indicator--;
			PlayerStates.DogFeedingState.stateTimers.IndicatorTimer.Reset();
		}
	}

	private void CheckSleepTimer()
	{
		if(IsSleeping)
		{
			if(PlayerStates.DogSleepState.stateTimers.ExtraTimer.ElapsedTime >= SleepSecondsToDecrease)
			{
				//Increse the sleep indicator by 2 (faster than decreasing, because resting is faster)
				PlayerStates.DogSleepState.Indicator += 2;
				PlayerStates.DogSleepState.stateTimers.ExtraTimer.Reset();
				PlayerStates.DogSleepState.stateTimers.IndicatorTimer.Reset();
			}

			if(PlayerStates.DogSleepState.Indicator >= 100)
				DoWakeUp();
		}
		else if(PlayerStates.DogSleepState.stateTimers.IndicatorTimer.ElapsedTime >= SleepSecondsToDecrease)
		{
			PlayerStates.DogSleepState.Indicator--;
			PlayerStates.DogSleepState.stateTimers.IndicatorTimer.Reset();
		}
	}

	private void CheckFunTimer()
	{
		if(PlayerStates.DogFunState.stateTimers.IndicatorTimer.ElapsedTime >= MoodSecondsToDecrease)
		{
			PlayerStates.DogFunState.Indicator--;
			PlayerStates.DogFunState.stateTimers.IndicatorTimer.Reset();
		}
	}

	private void CheckHygieneTimer()
	{
		if(PlayerStates.DogHygieneState.stateTimers.IndicatorTimer.ElapsedTime >= HygieneSecondsToDecrease)
		{
			PlayerStates.DogHygieneState.Indicator--;
			PlayerStates.DogHygieneState.stateTimers.IndicatorTimer.Reset();
		}
	}

	private void CheckHappinessTimer()
	{
		if(PlayerStates.DogHappinessState.stateTimers.IndicatorTimer.ElapsedTime >= HappinessSecondsToDecrease)
		{
			PlayerStates.DogHappinessState.Indicator--;
			PlayerStates.DogHappinessState.stateTimers.IndicatorTimer.Reset();
		}
	}
	#endregion
	#endregion

	#region State Change Listeners
	private void OnHydrationStateChange(Hydration newState)
	{
		PlayerStates.DogHydrationState.stateTimers.InsideStateTimer.Reset();
		if(newState == Hydration.Thirsty)
		{
			if(WaterBowl.HasWater && !BlockingActionsQueue.Contains(Actions.Drink))
				BlockingActionsQueue.Enqueue(Actions.Drink);
		}
	}

	private void OnFeedingStateChange(Feeding newState)
	{
		PlayerStates.DogFeedingState.stateTimers.InsideStateTimer.Reset();
		if(newState == Feeding.Hungry)
		{
			if(FoodBowl.HasFood && !BlockingActionsQueue.Contains(Actions.Eat))
				BlockingActionsQueue.Enqueue(Actions.Eat);
		}
	}

	private void OnSleepStateChange(Sleep newState)
	{
		PlayerStates.DogSleepState.stateTimers.InsideStateTimer.Reset();

		if(newState == Sleep.Asleep)
			BlockingActionsQueue.Enqueue(Actions.Sleep);
	}

    private void OnFunStateChange(Fun newState) => PlayerStates.DogFunState.stateTimers.InsideStateTimer.Reset();

    private void OnHygieneStateChange(Hygiene newState) => PlayerStates.DogHygieneState.stateTimers.InsideStateTimer.Reset();

    private void OnHappinessStateChange(Happiness newState) => PlayerStates.DogHappinessState.stateTimers.InsideStateTimer.Reset();

    private void OnPooLoadStateChange(ExcrementLoad newState)
	{
		if(newState == ExcrementLoad.LOAD_0)
		{
			PlayerStates.DogPooLoadState.stateTimers.ExtraTimer.Reset();
			PlayerStates.DogPooLoadState.stateTimers.ExtraTimer.Pause();
		}
		else if(PlayerStates.DogPooLoadState.stateTimers.ExtraTimer.IsStopped)
			PlayerStates.DogPooLoadState.stateTimers.ExtraTimer.Play();
	}

	private void OnPeeLoadStateChange(ExcrementLoad newState)
	{
		if(newState == ExcrementLoad.LOAD_0)
		{
			PlayerStates.DogPeeLoadState.stateTimers.ExtraTimer.Reset();
			PlayerStates.DogPeeLoadState.stateTimers.ExtraTimer.Pause();
		}
		else if(PlayerStates.DogPeeLoadState.stateTimers.ExtraTimer.IsStopped)
			PlayerStates.DogPeeLoadState.stateTimers.ExtraTimer.Play();
	}
	#endregion

	#region Automatic Actions
	public void OnActionEnded(Actions action)
	{
		IsDoingBlockingAction = false;

		switch(action)
		{
			case Actions.Drink: DrinkDone(); break;
			case Actions.Eat: EatDone(); break;
			case Actions.Pee: PeeDone(); break;
			case Actions.Poo: PooDone(); break;
			case Actions.Wake_up: WakeUpDone(); break;
		}
	}

	private void DoDrink()
	{
		Debug.Log("Drinking");
		IsDoingBlockingAction = true;
		GM.Inst.dogController.GoDrink();
	}

	private void DoEat()
	{
		Debug.Log("Eating");
		IsDoingBlockingAction = true;
		GM.Inst.dogController.GoEat();
	}

	private void DoPoo()
	{
		Debug.Log("Pooing");
		IsDoingBlockingAction = true;
		GM.Inst.dogController.Poo();
	}

	private void DoPee()
	{
		Debug.Log("Peeing");
		IsDoingBlockingAction = true;
		GM.Inst.dogController.Pee();
	}

	private void DoSleep()
	{
		// If the indicator is 0 -> sleep immediately. If the sim mode is Boredom, he can also go to sleep immediately
		if((PlayerStates.DogSleepState.Indicator <= 0) || (GM.Inst.simMode.ActiveMode == SimModeEnum.Boredom))
		{
			Debug.Log("Sleeping");
			GM.Inst.dogController.Sleep();
			IsSleeping = true;
			PlayerStates.DogSleepState.stateTimers.ExtraTimer.Play();
			PlayerStates.DogSleepState.stateTimers.IndicatorTimer.Pause();
			PlayerStates.DogSleepState.stateTimers.IndicatorTimer.Reset();
			PlayerStates.DogSleepState.stateTimers.InsideStateTimer.Pause();
			PlayerStates.DogSleepState.stateTimers.InsideStateTimer.Reset();
		}
		else
			BlockingActionsQueue.Enqueue(Actions.Sleep);
	}

	private void DoWakeUp()
	{
		Debug.Log("Waking up");
		IsSleeping = false;
		IsDoingBlockingAction = true;
		GM.Inst.dogController.WakeUp();
	}

	private void DoComeBack()
	{
		Debug.Log("Coming here!");
		GM.Inst.simMode.ActiveMode = SimModeEnum.Attention;
		GM.Inst.dogController.Goback();
	}

	private void DoMoveToRandomPosition()
	{
		Debug.Log("Random Position");
        IsDoingBlockingAction = true;
        GM.Inst.dogController.GoToRandomPosition();
	}

	private void DoMoveToRandomScratch()
	{
		Debug.Log("Random Scratch");
        IsDoingBlockingAction = true;
        GM.Inst.dogController.GoToRandomScratch();
	}

	private void DrinkDone()
	{
		PlayerStates.DogHydrationState.Indicator += 30;
		PlayerStates.DogHappinessState.Indicator += 5;
		PlayerStates.DogPeeLoadState.ActualState++;
		WaterBowl.HasWater = false;
		Debug.Log("Drink done");
	}

	private void EatDone()
	{
		PlayerStates.DogFeedingState.Indicator += 30;
		PlayerStates.DogHappinessState.Indicator += 5;
		PlayerStates.DogPooLoadState.ActualState++;
		FoodBowl.HasFood = false;
		Debug.Log("Eat done");
	}

	private void PooDone()
	{
		PlayerStates.DogFeedingState.Indicator += 30;
		PlayerStates.DogHappinessState.Indicator += 5;
		PlayerStates.DogPooLoadState.ActualState = ExcrementLoad.LOAD_0;
	}

	private void PeeDone()
	{
		PlayerStates.DogHygieneState.Indicator -= (int)PlayerStates.DogPeeLoadState.ActualState * Defines.PeeHygieneDecreaser;
		PlayerStates.DogHappinessState.Indicator += 5;
		PlayerStates.DogPeeLoadState.ActualState = ExcrementLoad.LOAD_0;
	}

	private void WakeUpDone()
	{
		//Every SleepToHappinessNeed seconds that elapsed sleeping, we add one point to happiness
		PlayerStates.DogHappinessState.Indicator += (int)PlayerStates.DogSleepState.stateTimers.ExtraTimer.ElapsedTime / Defines.SleepToHappinessNeeded;
		PlayerStates.DogSleepState.stateTimers.ExtraTimer.Pause();
		PlayerStates.DogSleepState.stateTimers.ExtraTimer.Reset();
		PlayerStates.DogSleepState.stateTimers.IndicatorTimer.Play();
		PlayerStates.DogSleepState.stateTimers.InsideStateTimer.Play();
	}
	#endregion

	#region Boredom actions
	private void DoSniff()
	{
		Debug.Log("Sniffing");
		PlayerStates.DogFunState.Indicator -= Defines.BoredomDecreaser;

        IsDoingBlockingAction = true;
        GM.Inst.dogController.GoSniff();
    }

	private void DoScratchWall()
	{
		Debug.Log("Scratching Wall");
		PlayerStates.DogFunState.Indicator -= Defines.BoredomDecreaser;

        IsDoingBlockingAction = true;
        GM.Inst.dogController.GoToRandomScratch();
    }

	private void DoBark()
	{
		Debug.Log("Barking");

        IsDoingBlockingAction = true;
        GM.Inst.dogController.Bark();
    }
	#endregion

	#region Actions Logic
	private void CycleBoredomOrAttention()
	{
		if(GM.Inst.simMode.ActiveMode == SimModeEnum.Boredom)
		{
			if(BoredomTimer.IsStopped)
			{
				BoredomTimer.Reset();
				BoredomTimer.Play();
			}

			BoredomBehaviourUpdate();
			HandleAutomaticActionQueue();
		}
		else if(GM.Inst.simMode.ActiveMode == SimModeEnum.Attention)
		{
			if(!BoredomTimer.IsStopped)
				BoredomTimer.Pause();
		}
	}

	private void HandleAutomaticActionQueue()
	{
		if(!IsDoingBlockingAction && !IsSleeping && BlockingActionsQueue.Count > 0)
		{
			switch(BlockingActionsQueue.Dequeue())
			{
				case Actions.Bark: DoBark(); break;
				case Actions.Eat: DoEat(); break;
				case Actions.Drink: DoDrink(); break;
				case Actions.Pee: DoPee(); break;
				case Actions.Poo: DoPoo(); break;
				case Actions.Scratch_wall: DoScratchWall(); break;
				case Actions.Sniff: DoSniff(); break;
				case Actions.Sleep: DoSleep(); break;
				case Actions.RandomWander: DoMoveToRandomPosition(); break;
				case Actions.RandomScratch: DoMoveToRandomScratch(); break;
			}
		}
	}

	private void PeePooBehaviourUpdate()
	{
		if((int)PlayerStates.DogPooLoadState.ActualState > 0 && PlayerStates.DogPooLoadState.stateTimers.ExtraTimer.ElapsedTime > TimeToPeeAndPooWhenLoaded)
		{
			if(!BlockingActionsQueue.Contains(Actions.Poo))
				BlockingActionsQueue.Enqueue(Actions.Poo);
		}

		if((int)PlayerStates.DogPeeLoadState.ActualState > 0 && PlayerStates.DogPeeLoadState.stateTimers.ExtraTimer.ElapsedTime > TimeToPeeAndPooWhenLoaded)
		{
			if(!BlockingActionsQueue.Contains(Actions.Pee))
				BlockingActionsQueue.Enqueue(Actions.Pee);
		}
	}

	private void BoredomBehaviourUpdate()
	{
		if(IsDoingBlockingAction)
			return;

		if(BoredomTimer.ElapsedTime >= TimeBetweenBoredomActions)
		{
			BoredomTimer.Reset();
			BoredomActionCounter++;

			if(BoredomActionCounter > 3)
			{
				BlockingActionsQueue.Enqueue(Actions.RandomScratch);
				BoredomActionCounter = 0;
			}
			else
				BlockingActionsQueue.Enqueue(Actions.RandomWander);
		}
	}

	public void AddDrinkActionIfNeeded()
	{
		if(PlayerStates.DogHydrationState.ActualState == Hydration.Thirsty)
		{
			if(WaterBowl.HasWater && !BlockingActionsQueue.Contains(Actions.Drink))
				BlockingActionsQueue.Enqueue(Actions.Drink);
		}
	}

	public void AddEatActionIfNeeded()
	{
		if(PlayerStates.DogFeedingState.ActualState == Feeding.Hungry)
		{
			if(FoodBowl.HasFood && !BlockingActionsQueue.Contains(Actions.Eat))
				BlockingActionsQueue.Enqueue(Actions.Eat);
		}
	}
    #endregion

    #region Test methods
    public void AddDrinkActionToQueue() => BlockingActionsQueue.Enqueue(Actions.Drink);
    public void AddEatActionToQueue() => BlockingActionsQueue.Enqueue(Actions.Eat);
    public void AddPooActionToQueue() => BlockingActionsQueue.Enqueue(Actions.Poo);
    public void AddPeeActionToQueue() => BlockingActionsQueue.Enqueue(Actions.Pee);
    public void DoEatTest() => DoEat();
    public void DoDrinkTest() => DoDrink();
    public void DoPooTest() => DoPoo();
    public void DoPeeTest() => DoPee();
    #endregion
}
