using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EZObjectPools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

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
        EndGame,
        GotoMenu,
    };

    public GameObject block;
    public HSM hsm;
    public ChainMatchController chainMatchController;
    public Text scoreText;
    public Text movesText;
    public Image resultImage;
    public Text resultText;

    private List<int> levelData = new List<int>();
    private List<BlockBehaviour> blocksData = new List<BlockBehaviour>();
    private Sound bgMusic;
    private EZObjectPool blockPool;
    private int score = 0;
    private int movesRemaining = 20;
    private int totalMoves = 20;
    private bool warned = false;
    // Use this for initialization
    void Start () {
        resultImage.transform.DOMoveX(2000, 0.1f);
        InitStateTransition();
        Invoke("StartLater", 0.5f);
        bgMusic = AudioManager.Main.NewSound("bg_music", loop: true, interrupts: false);
        bgMusic.PlayOrPause(true, false); 
    }
    void StartLater()
    {
        blockPool = EZObjectPool.CreateObjectPool(block.gameObject, "Blocks", Utils.width * Utils.height * 2, true, true, false);
        ReGenerate();
        hsm.Go(State.Validate_Board);
        movesRemaining = totalMoves;
    }

    public void ReGenerate()
    {
        AudioManager.Main.PlayNewSound("falldown");
        Utils.PopulateGrid(ref blockPool, ref blocksData, ref levelData);
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
        hsm.AddTransition(new KeyValuePair<State, State>(State.Idle, State.Validate_Board), ToValidateBoard);
        hsm.AddTransition(new KeyValuePair<State, State>(State.Idle, State.EndGame), ToEndGame);
        hsm.AddTransition(new KeyValuePair<State, State>(State.EndGame, State.GotoMenu), GotoMenu);
    }
    void ToEndGame()
    {
        StartCoroutine(PlayEndGame());
    }
    IEnumerator SetMessage(string msg, float delay)
    {
        resultText.text = msg;
        Vector3 pos = resultImage.transform.position;
        pos.x = 2000;
        resultImage.transform.position = pos;
        yield return new WaitForSeconds(0.1f);
        resultImage.transform.DOMoveX(0, 0.5f);
        yield return new WaitForSeconds(delay);
        resultImage.transform.DOMoveX(-2000, 0.25f);
        yield return new WaitForSeconds(0.25f);
    }

    IEnumerator PlayEndGame()
    {
        string result = string.Format("Good Job ! You have scored {0} points in {1} moves", score, totalMoves);
        StartCoroutine(SetMessage(result, 5.0f));
        yield return new WaitForSeconds(5.5f);
        hsm.Go(State.GotoMenu);
    }
    void ToIdle()
    {
        if (movesRemaining == 5 && !warned)
        {
            string result = string.Format("You Have 5 Moves Left");
            StartCoroutine(SetMessage(result, 3.0f));
            warned = true;
        }
        else if (movesRemaining <= 0)
        {
            hsm.Go(State.EndGame);
        }
    }
    public void ToValidMatch()
    {
        movesRemaining--;
        movesText.text = movesRemaining.ToString();
        hsm.Go(State.Remove_Items);
    }
    public void ToInvalid()
    {
        hsm.Go(State.Idle);
    }
    public void ToRemoveItems()
    {
        List<BlockBehaviour> chainList = chainMatchController.GetMatchedChain();
        float delay = 0;
        foreach (BlockBehaviour blk in chainList)
        {
            levelData[blk.info.Id]= 0;
            delay += 0.1f;
            StartCoroutine(Disappear(blk, delay));
        }
        StartCoroutine(MoveDown(chainList, delay));
    }
    IEnumerator Disappear(BlockBehaviour blk, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.Main.PlayNewSound("dispear");
        blocksData[blk.info.Id].SetGem(0);
        blk.HideItem();
        score += (int)(500.0f * delay);
        scoreText.text = score.ToString();
    }
    IEnumerator MoveDown(List<BlockBehaviour> chainList, float delay)
    {
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
        AudioManager.Main.PlayNewSound("falldown");
        yield return new WaitForSeconds(delay);
        Shrink();

        foreach (BlockBehaviour blk in blocksData)
        {
            blk.MoveDown();
        }
        yield return new WaitForSeconds(0.25f);
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
        AudioManager.Main.PlayNewSound("falldown");
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
    void ChainCount(int row, int col, long old, ref int count)
    {
        if ((row < 0) || (row >= Utils.height)) return;
        if ((col < 0) || (col >= Utils.width)) return;
        if (count > 2) return;
        int id = Utils.GetID(row, col);
        if (levelData[id] == old)
        {
            count++;
            ChainCount(row + 1, col, old, ref count);
            ChainCount(row - 1, col, old, ref count);
            ChainCount(row, col + 1, old, ref count);
            ChainCount(row, col - 1, old, ref count);
            ChainCount(row + 1, col + 1, old, ref count);
            ChainCount(row + 1, col - 1, old, ref count);
            ChainCount(row - 1, col + 1, old, ref count);
            ChainCount(row - 1, col - 1, old, ref count);
        }
    }


    public void ToValidateBoard()
    {
        for (int i = 0; i < levelData.Count; i++)
        {
            int row = 0, col = 0, count = 0;
            Utils.GetRowCol(i, out row, out col);
            ChainCount(row, col,levelData[i], ref count);
            if (count > 2)
            {
                hsm.Go(State.Idle);
                return;
            }
        }
        hsm.Go(State.Shuffle_Board);
    }
    public void ToShuffleBoard()
    {
        ReGenerate();
        hsm.Go(State.Validate_Board);
    }
    public void GotoMenu()
    {
        SceneManager.LoadScene("menu");
    }
}
