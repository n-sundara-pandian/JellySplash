using UnityEngine;
using System.Collections.Generic;

public class MovesGPController : GamePlayController
{
	public int TotalMoves;
	public int CurrentMove;
	int RemainingSteps;
	public override void init (Dictionary<string, int> param)
	{
		TotalMoves = param ["moves"];
		CurrentMove = 0; 
		Notify ("controller.tick");
	}
	public override void tick (){ 
		CurrentMove++; 
		RemainingSteps = TotalMoves - CurrentMove;
		Notify ("controller.tick");
		if (RemainingSteps <= 0) {
			Notify ("controller.gameover");
		} else if (RemainingSteps == 5) {
			Notify ("controller.warn");
		}
	}
	public override int GetRemainingSteps (){return RemainingSteps;}
	public override int GetTotalMoves() { return TotalMoves;}
	public override void Pause(float delay) {}
	public override void Resume(float delay) {}
	public override void AddSteps(int n) { RemainingSteps += n; }

}