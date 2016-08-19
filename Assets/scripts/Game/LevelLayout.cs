﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EZObjectPools;
public class LevelLayout : MonoBehaviour {

    public enum State
    {
        Idle,
        Valid_match,
        Invalid_Match,
        Remove_Items,
        Fill_Items,
        Validate_Board,
        Shuffle_Board,
    };

    public GameObject block;
    public List<int> levelData = new List<int>();
    public List<BlockBehaviour> blocksData = new List<BlockBehaviour>();
    private EZObjectPool blockPool;
    public ChainMatchController chainMatchController;

    public HSM hsm;

    // Use this for initialization
    void Start () {
        InitStateTransition();
        Invoke("StartLater", 0.5f);
    }
    void StartLater()
    {
        blockPool = EZObjectPool.CreateObjectPool(block.gameObject, "Blocks", Utils.width * Utils.height * 2, true, true, false);
        Utils.DetermineGrid(ref blockPool, ref blocksData, ref levelData);
    }

    void InitStateTransition()
    {
        hsm.AddTransition(new KeyValuePair<State, State>(State.Idle, State.Valid_match), ToValidMatch);
        hsm.AddTransition(new KeyValuePair<State, State>(State.Idle, State.Invalid_Match), ToInvalid);
        hsm.AddTransition(new KeyValuePair<State, State>(State.Invalid_Match, State.Idle), ToIdle);
        hsm.AddTransition(new KeyValuePair<State, State>(State.Valid_match, State.Remove_Items), ToRemoveItems);
        hsm.AddTransition(new KeyValuePair<State, State>(State.Remove_Items, State.Fill_Items), ToFillItems);
        hsm.AddTransition(new KeyValuePair<State, State>(State.Fill_Items, State.Validate_Board), ToValidateBoard);
        hsm.AddTransition(new KeyValuePair<State, State>(State.Validate_Board, State.Shuffle_Board), ToShuffleBoard);
        hsm.AddTransition(new KeyValuePair<State, State>(State.Shuffle_Board, State.Validate_Board), ToValidateBoard);
        hsm.AddTransition(new KeyValuePair<State, State>(State.Validate_Board, State.Idle), ToIdle);
    }

    public void ToIdle()
    {
        Debug.Log("ToIdle Called");
    }
    public void ToValidMatch()
    {
        hsm.Go(State.Remove_Items);
    }
    public void ToInvalid()
    {
        hsm.Go(State.Idle);
    }
    public void ToRemoveItems()
    {
        List<BlockBehaviour> chainList = chainMatchController.GetMatchedChain();
        foreach (BlockBehaviour blk in chainList)
        {
            levelData[blk.info.Id]= 0;
            blocksData[blk.info.Id].SetGem(0);
            blk.HideItem();
        }
        foreach (BlockBehaviour blk in chainList)
        {
            int id = blk.info.Id;
            int above_blocks_count = 0;
            for (int i = id; i >= 0; i -= Utils.width)
            {
                above_blocks_count++;
                if (levelData[i] != 0)
                {
                    blocksData[i].SetMoveDown(1);
                }
            }
        }
        StartCoroutine(MoveDown(chainList, 0.25f));
    }

    IEnumerator MoveDown(List<BlockBehaviour> chainList, float delayTime)
    {

        Shrink();

        foreach (BlockBehaviour blk in blocksData)
        {
            blk.MoveDown();
        }
        yield return new WaitForSeconds(delayTime);
        for (int i = blocksData.Count - 1; i >= 0; --i)
        {
            blocksData[i].SetGem(levelData[i]);
            blocksData[i].UpdatePosition();
        }
        hsm.Go(State.Fill_Items);
    }
    public void Shrink()
    {
        for (int i = Utils.width - 1; i >= 0; --i)
        {
            int ktop = Utils.height - 1;
            for (int j = ktop; j >= 0; --j)
            {
                if (levelData[Utils.GetID(j, i)] == 0)
                {
                    for (int k = j - 1; k >= 0; --k)
                    {
                        if (levelData[Utils.GetID(k, i)] != 0)
                        {
                            levelData[Utils.GetID(j, i)] = levelData[Utils.GetID(k, i)];
                            levelData[Utils.GetID(k, i)] = 0;
                            ktop = j;
                            break;
                        }
                    }
                }
                else
                {
                    ktop = j;
                }
            }
        }
    }

    public void ToFillItems()
    {
        List<BlockBehaviour> emptyChainList = new List<BlockBehaviour>();
        for (int i = 0; i < blocksData.Count; i++)
        {
            if (levelData[i] == 0)
            {
                emptyChainList.Add(blocksData[i]);
                blocksData[i].UpdatePosition(-10);
                levelData[i] = Utils.GetValidGem();
                blocksData[i].SetGem(levelData[i]);
                blocksData[i].GetIntoPosition();
            }
        }
        hsm.Go(State.Validate_Board);
    }

    public void ToValidateBoard()
    {
        hsm.Go(State.Idle);
        Debug.Log("ToValidateBoard Called");
    }
    public void ToShuffleBoard()
    {
        Debug.Log("ToShuffleBoard Called");
    }
}
