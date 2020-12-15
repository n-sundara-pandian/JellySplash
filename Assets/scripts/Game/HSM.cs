using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HSM : MonoBehaviour {
    public enum State
    {
        Start,
        Init,
        Idle,
        Matching,
        Valid_match,
        Invalid_Match,
        Remove_Items,
        Fill_Items,
        Validate_Board,
        Shuffle_Board,
        EndGame,
        GotoMenu,
		FloodFill,
    };

    Dictionary<KeyValuePair<State, State>, Action> TransitionMap = new Dictionary<KeyValuePair<State, State>, Action>();
    State currentState = State.Start;

    public void AddTransition(KeyValuePair<State, State> key, Action action)
    {
        TransitionMap[key] = action;
    }
    IEnumerator transit(KeyValuePair<State, State> key, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        TransitionMap[key]();
    }
    public void CheckAndGo(State nextState, float delay = 0.1f)
    {
        if (currentState == nextState) return;
        Go(nextState, delay);
    }

    public void Go(State nextState, float delay = 0.1f)
    {
        KeyValuePair<State, State> key = new KeyValuePair<State, State>(currentState, nextState);
        if (TransitionMap.ContainsKey(key))
        {
            currentState = nextState;
            StartCoroutine(transit(key, delay));
        }
        else
        {
            Debug.Log("Failed To Transit From " + currentState.ToString() +  " to " + nextState.ToString());
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentState = State.Idle;
        }
    }

    public State GetCurrentState() { return currentState; }	
}
