using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;
using UnityEngine.SceneManagement;

public class GameController : Controller<Game> {
    public HSM hsm;
    private ChainMatchController matchController;
	bool GameOver = false;
    void Start()
    {
        Invoke("StartLater", 0.5f);
    }

    void StartLater()
    {
        InitStateMap();
        hsm.Go(HSM.State.Init);
		GameOver = false;
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
		hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Idle, HSM.State.FloodFill), ToFloodFill);
		hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.FloodFill, HSM.State.Valid_match), ToValidMatch);
		hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.FloodFill, HSM.State.Invalid_Match), ToInvalid);
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
		case "controller.tick":
                {
                    app.view.SetValue("move", app.model.GetRemainingMoves());
                    break;
                }
            case "timerup":
                {
                    GameOver = true;
                    if (hsm.GetCurrentState() == HSM.State.Idle)
                    {
                        hsm.Go(HSM.State.EndGame);
                    }
                    break;
                }
		case "controller.gameover":
			{
				GameOver = true;
				break;
			}
		case "controller.warn":
			{
				if(!app.view.warned)
				{
				//	app.view.ShowMessage(" 5 Seconds Remaining", 3.0f);
					app.view.warned = true;
				}
				break;
			}
            case "show.wincondition":
                {
                    string message = string.Format(" Score {1} points in {0} Seconds", app.model.levels.timer, app.model.levels.targetScore);
                    app.view.ShowMessage(message, 2.0f);
                    app.model.gamePlayController.Pause(0.5f);
                    break;
                }
        case "contoller.summary":
            {
                
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
		Assert<ChainMatchController>(matchController).Reset ();
		if (GameOver)
			hsm.Go(HSM.State.EndGame);		
    }
    void ToHorizontalLine()
    {
        int lastBlockId = Assert<ChainMatchController>(matchController).GetLastBlockID();
        app.model.FloodFill(lastBlockId);
        if (app.model.FillItemList.Count > 2)
        {
            Assert<ChainMatchController>(matchController).chainList.Clear();
            for (int i = 0; i < app.model.FillItemList.Count; i++)
            {
                Assert<ChainMatchController>(matchController).chainList.Add(app.view.GetBlockForID(app.model.FillItemList[i]));
            }
            hsm.Go(HSM.State.Valid_match);
        }
        else
            hsm.Go(HSM.State.Invalid_Match);

    }
    void ToFloodFill()
	{
		int  lastBlockId = Assert<ChainMatchController>(matchController).GetLastBlockID();
	
		app.model.FloodFill (lastBlockId);
		if (app.model.FillItemList.Count > 2) 
		{
			Assert<ChainMatchController>(matchController).chainList.Clear ();
			for (int i = 0; i < app.model.FillItemList.Count; i++) {
				Assert<ChainMatchController>(matchController).chainList.Add (app.view.GetBlockForID (app.model.FillItemList[i]));
			}
			hsm.Go (HSM.State.Valid_match);
		}
		else
			hsm.Go (HSM.State.Invalid_Match);

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
            app.view.HideItem(blk, (float)count * 0.1f);
            count++;
        }
        app.model.SetStreak(count);
        app.view.FallDown(app.model.GetBoard(), chainList, count * 0.1f);
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
        if (app.model.IsTargetAchieved())
        {
            GotoWin();
        }
        else
        {
            app.view.LoseGame();
        }
        
    }
    void GotoMenu()
    {
        SceneManager.LoadScene("menu1");
    }
    void GotoWin()
    {
        SceneManager.LoadScene("menu1");
    }
}
