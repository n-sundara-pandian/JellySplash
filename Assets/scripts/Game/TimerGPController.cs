using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimerGPController : GamePlayController {

	public int LevelTimeInSec;
	public int ElapsedTime;
	int RemainingTime;
	Timer GameTimer;
	bool bGameOver;
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
			});			
	}
	public override void tick (){ }
	public override int GetRemainingSteps (){return RemainingTime;}
	public override int GetTotalMoves() { return LevelTimeInSec;}
	public override void Pause() { GameTimer.Pause ();}
	public override void Resume() { GameTimer.Resume ();}

	void OnTimeComplete() { bGameOver = true;Notify ("controller.gameover");}

}
