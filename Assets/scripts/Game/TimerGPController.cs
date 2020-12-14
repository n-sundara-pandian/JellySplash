using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimerGPController : GamePlayController {

	Timer GameTimer;
	public override void init (Dictionary<string, int> param)
	{
		int LevelTimeInSec = param ["timer"];
		GameTimer = Timer.Register (LevelTimeInSec, OnTimeComplete, onUpdate: secondsElapsed =>	
			{
				Notify ("controller.tick");
				int RemainingTime = (int)(GameTimer.duration - secondsElapsed);
				if (RemainingTime == 5) {
					Notify ("controller.warn");
				}
                if (RemainingTime <= 0)
                {
                    Notify("timerup");
                }
			});
        Notify("show.wincondition");
        Resume(3.0f);
	}
	public override void tick (){ }
	public override int GetRemainingSteps (){return (int)(GameTimer.duration - GameTimer.GetTimeElapsed());}
	public override int GetTotalMoves() { return (int)GameTimer.duration;}
	public override void Pause(float delay) {
        if (delay == 0)
            PauseTimer();
        else
            Invoke("PauseTimer", delay);
    }
	public override void Resume(float delay)
    {
        if (delay == 0)
            ResumeTimer();
        else
            Invoke("ResumeTimer", delay);
    }

    void PauseTimer() { GameTimer.Pause(); }
    void ResumeTimer() { GameTimer.Resume(); }

	void OnTimeComplete() { Notify ("controller.gameover");}

	public override void AddSteps(int n) { GameTimer.AddTime(n); }

}
