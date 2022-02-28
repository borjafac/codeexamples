public class StateTimers
{
	public Timer IndicatorTimer;
	public Timer InsideStateTimer;
	public Timer ExtraTimer;

	public StateTimers()
	{
		IndicatorTimer = GM.Inst.timerFactory.CreateTimer();
		InsideStateTimer = GM.Inst.timerFactory.CreateTimer();
		ExtraTimer = GM.Inst.timerFactory.CreateTimer();
	}
}
