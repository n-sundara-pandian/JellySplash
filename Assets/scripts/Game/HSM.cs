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
            Console.Write("Could not transit LevelLayout.State " + Environment.NewLine);
        }
    }

    public LevelLayout.State GetCurrentState() { return currentState; }	
}
