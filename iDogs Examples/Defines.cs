using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRubyShared;

#region Enums

#region States
public enum Hydration { Thirsty, Normal, Hydrated };
public enum Feeding { Hungry, Normal, Fed };
public enum Sleep { Asleep, Sleepy, Awake };
public enum Fun { Bored, Contented, Cheerful };
public enum Hygiene { Dirty, Clean, Shiny };
public enum Happiness { Sad, Normal, Happy };
public enum ExcrementLoad { LOAD_0, LOAD_1, LOAD_2, LOAD_3, LOAD_4 }
#endregion

public enum Tricks { Sit, Roll_left, Roll_right, Play_dead, Lay_down, Pickup_object, Give_paw, Backflip };
public enum Actions { Pee, Poo, Sleep, Sniff, Scratch_wall, Bark, Pet, Wash, Drink, Eat, Do_trick, Wake_up, Sit, Jaw, Hit_pawr, Liedown, RandomWander, RandomScratch, Null, GrabBall, ReturningBall };
public enum SimModeEnum { Furniture, Boredom, Attention };
public enum FurnitureType { Table, Chair, DogsBed, Kitchen, Carpet, Couch }
public enum Tags { Item, Furniture, Throwable, Head, Neck, WaterBowl, FoodBowl, MainCamera, Poo, Pee }
public enum ButtonsType { Cloth, Accessory, Food, Toy, Bed, Furniture, WallPaint }
public enum AccessoryType { Chest, Head, Feet }
public enum UITags { FurnitureUI, FoodBowlUI, WaterBowlUI, InventoryUI, ItemInteractionUI, OptionsUI, IngameBasicUI, ShopUI, EndGameUI, CleanPeePooUI }
public enum EffectorType { Offset, InteractionObject, CCDIK, FBBIK }

#endregion

public static class Defines
{
	//Constants Anim Strings
	public const string State_Idle = "Idle";
	public const string State_Sleeping = "Sleeping";

	public const string Trigger_Poo = "Poo";
	public const string Trigger_Pee = "Pee";
	public const string Trigger_Sleep = "Sleep";
	public const string Trigger_WakeUp = "Wake_up";
	public const string Trigger_LieDown = "liedown";
	public const string Trigger_Sit = "Sit";
    public const string Trigger_Drink = "Drink";
    public const string Trigger_Eat = "Eat";
	public const string Trigger_Jaw = "Jaw";
	public const string Trigger_Howling = "howling";
	public const string Trigger_Alerted = "alerted";
	public const string Trigger_AlertedJump = "alertedJump";
	public const string Trigger_AttackPawR = "Hit_pawr";
	public const string Trigger_AttackTwoPaw = "AttackTwoPaw";
	public const string Bool_AlertedFocus = "alertedFocus";

	//Indicator max parameter
	public const int MaxParameterIndicator = 100;
	public const int PooHygieneDecreaser = 10;
	public const int PeeHygieneDecreaser = 10;
	public const int BoredomDecreaser = 5;

	//Time needed to increment one time happiness thanks to being slept
	public const int SleepToHappinessNeeded = 5;

	public const float TimeToCheckDoubleTap = 0.5f;
}
