using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;

interface IGameCondition
{
	void init (Dictionary<string, int> param);
	void tick ();
	int GetRemainingSteps ();
}

public abstract class GamePlayController : Controller<Game>, IGameCondition {
	public abstract void init (Dictionary<string, int> param);
	public abstract void tick ();
	public abstract int GetRemainingSteps ();
	public abstract int GetTotalMoves ();
	public abstract void Pause();
	public abstract void Resume ();
}

