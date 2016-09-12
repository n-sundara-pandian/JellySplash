﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;
using UnityEngine.SceneManagement;

public class GameController : Controller<Game> {
    public HSM hsm;
    private ChainMatchController matchController;
    void Start()
    {
        Invoke("StartLater", 0.5f);
    }

    void StartLater()
    {
        InitStateMap();
        hsm.Go(HSM.State.Init);
    }
    void InitStateMap()
    {
        hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Start, HSM.State.Init), ToInitBoard);
        hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Init, HSM.State.Validate_Board), ToValidateBoard);
        hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Idle, HSM.State.Invalid_Match), ToInvalid);
        hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Idle, HSM.State.Valid_match), ToValidMatch);
        hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Invalid_Match, HSM.State.Idle), ToIdle);
        hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Valid_match, HSM.State.Remove_Items), ToRemoveItems);
        hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Remove_Items, HSM.State.Fill_Items), ToFillItems);
        hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Fill_Items, HSM.State.Validate_Board), ToValidateBoard);
        hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Validate_Board, HSM.State.Shuffle_Board), ToShuffleBoard);
        hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Shuffle_Board, HSM.State.Validate_Board), ToValidateBoard);
        hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Validate_Board, HSM.State.Idle), ToIdle);
        hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Idle, HSM.State.Validate_Board), ToValidateBoard);
        hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Idle, HSM.State.EndGame), ToEndGame);
        hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.EndGame, HSM.State.GotoMenu), GotoMenu);
    }

    public override void OnNotification(string p_event, Object p_target, params object[] p_data)
    {
        switch (p_event)
        {
            case "scene.load":
                Log("Scene [" + p_data[0] + "][" + p_data[1] + "] loaded");
                break;
            case "view.fill_items":
                hsm.Go(HSM.State.Fill_Items);
                break;
            case "view.OnEndGameComplete":
                hsm.Go(HSM.State.GotoMenu);
                break;
            case "model.streak":
                {
                    string msg = string.Format(" Well done. this is ur longest streak at {0}", p_data[0]);
                    app.view.ShowMessage(msg, 3.0f);
                    Utils.SavePref("longestChain", (int)p_data[0]);
                    break;
                }
            case "model.highscore":
                {
                    if (!app.view.congradulated)
                    {
                        string msg = string.Format(" Nice. this is your high score");
                        app.view.ShowMessage(msg, 3.0f);
                        app.view.congradulated = true;
                    }
                    Utils.SavePref("highScore", (int)p_data[0]);
                    break;
                }
        }
    }
    void ToInitBoard()
    {
        app.model.Init(Utils.highScore, Utils.longestChain);
        app.model.PopulateBoard();
        app.view.InitBoardGfx(app.model.GetBoard());
        hsm.Go(HSM.State.Validate_Board);
    }
    void ToIdle()
    {
        app.view.SetValue("move", app.model.GetRemainingMoves());
        if (app.model.GetRemainingMoves() <= 0)
        {
            hsm.Go(HSM.State.EndGame);
        }
        else if (app.model.GetRemainingMoves() == 5)
        {
            if(!app.view.warned)
            {
                app.view.ShowMessage(" 5 Moves Remaining", 3.0f);
                app.view.warned = true;
            }
        }
    }
    void ToValidMatch()
    {
        app.model.DecMoves();
        app.view.SetValue("move", app.model.GetRemainingMoves());
        hsm.Go(HSM.State.Remove_Items);
    }
    void ToInvalid()
    {
        hsm.Go(HSM.State.Idle);
    }
    void ToRemoveItems()
    {
        List<BlockBehaviour> chainList = Assert<ChainMatchController>(matchController).chainList;
        int count = 1;
        foreach (BlockBehaviour blk in chainList)
        {
            app.model.SetValue(blk.info.Id, 0);
            app.model.AddScore(count);
            app.view.SetValue("score", app.model.GetScore());
            app.view.HideItem(blk, (float)count * 0.25f);
            count++;
        }
        app.model.SetStreak(count);
        app.view.FallDown(app.model.GetBoard(), chainList, count * 0.25f);
        app.model.Shrink();
    }
    public void ToFillItems()
    {
        app.model.FillItems();
        app.view.FillItems(app.model.GetBoard());
        hsm.Go(HSM.State.Validate_Board);
    }
    void ToValidateBoard()
    {
        if (app.model.ValidateBoard())
            hsm.Go(HSM.State.Idle);
        else
            hsm.Go(HSM.State.Shuffle_Board);
    }
    void ToShuffleBoard()
    {
        app.model.PopulateBoard();
        hsm.Go(HSM.State.Validate_Board);
    }
    void ToEndGame()
    {
        app.view.EndGame(app.model.GetTotalMoves(), app.model.GetScore());
    }
    void GotoMenu()
    {
        SceneManager.LoadScene("menu");
    }

}
