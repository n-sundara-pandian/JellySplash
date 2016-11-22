using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimerGPController : GamePlayController {

	public int LevelTimeInSec;
	public int ElapsedTime;
	int RemainingTime;
	Timer GameTimer;
    bool bGameOver = false;
	public override void init (Dictionary<string, int> param)
	{
		LevelTimeInSec = param ["timer"];
		ElapsedTime = 0; 
		bGameOver = false;
		GameTimer = Timer.Register (LevelTimeInSec, OnTimeComplete, onUpdate: secondsElapsed =>	
			{
				ElapsedTime= (int)secondsElapsed;
				Notify ("controller.tick");
				RemainingTime = LevelTimeInSec - ElapsedTime;
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
	public override int GetRemainingSteps (){return RemainingTime;}
	public override int GetTotalMoves() { return LevelTimeInSec;}
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

	void OnTimeComplete() { bGameOver = true;Notify ("controller.gameover");}

}
