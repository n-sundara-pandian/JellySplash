using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface IGameCondition
{
	void init (Dictionary<string, int> param);
	void tick ();
	bool GameOver ();
	int GetRemainingSteps ();
}

public abstract class GamePlayController : MonoBehaviour, IGameCondition {
	public abstract void init (Dictionary<string, int> param);
	public abstract void tick ();
	public abstract int GetRemainingSteps ();
	public abstract bool GameOver ();
	public abstract int GetTotalMoves ();
}

