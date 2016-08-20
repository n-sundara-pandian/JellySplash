using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HSM : MonoBehaviour {
    Dictionary<KeyValuePair<LevelLayout.State, LevelLayout.State>, Action> TransitionMap = new Dictionary<KeyValuePair<LevelLayout.State, LevelLayout.State>, Action>();
    LevelLayout.State currentState = LevelLayout.State.Idle;
    public void AddTransition(KeyValuePair<LevelLayout.State, LevelLayout.State> key, Action action)
    {
        TransitionMap[key] = action;
    }
    IEnumerator transit(KeyValuePair<LevelLayout.State, LevelLayout.State> key, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        TransitionMap[key]();
        Debug.Log(currentState.ToString());
        // Now do your thing here
    }
    public void Go(LevelLayout.State nextState, float delay = 0.1f)
    {
        KeyValuePair<LevelLayout.State, LevelLayout.State> key = new KeyValuePair<LevelLayout.State, LevelLayout.State>(currentState, nextState);
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

    public LevelLayout.State GetCurrentState() { return currentState; }	
}
