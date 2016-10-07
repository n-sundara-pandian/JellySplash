using UnityEngine;
using System.Collections.Generic;

public class MovesGPController : GamePlayController
{
	public int TotalMoves;
	public int CurrentMove;

	public override void init (Dictionary<string, int> param)
	{
		TotalMoves = param ["moves"];
		CurrentMove = 0; 
	}
	public override void tick (){ CurrentMove++;}
	public override int GetRemainingSteps (){return TotalMoves - CurrentMove;}
	public override bool GameOver (){return CurrentMove < TotalMoves;}
	public override int GetTotalMoves() { return TotalMoves;}
}