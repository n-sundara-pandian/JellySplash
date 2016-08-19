using UnityEngine;
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

    public Transform TopLeft;
    public Transform BottomRight;
    public GameObject block;
    public List<int> levelData = new List<int>();
    public List<BlockBehaviour> blocksData = new List<BlockBehaviour>();
    private Vector3 StartOffset;
    private EZObjectPool blockPool;
    public ChainMatchController chainMatchController;

    public HSM hsm;

    // Use this for initialization
    void Start () {
        blockPool = EZObjectPool.CreateObjectPool(block.gameObject, "Blocks", Utils.width * Utils.height * 2, true, true, false);
        InitStateTransition();
        DetermineGrid();
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

    public void DetermineGrid()
    {
        int min_img_size = 32;
        int max_img_size = 256;
        blockPool.DeActivatePool();
        levelData.Clear();
        blocksData.Clear();
        float totalAvailableWidth = Mathf.Abs(TopLeft.position.x - BottomRight.position.x);
        float totalAvailableHeight = Mathf.Abs(TopLeft.position.y - BottomRight.position.y);
        float mid_x = (TopLeft.position.x + totalAvailableWidth) / 2.0f;
        int suggested_img_size = min_img_size;
        for (int i = min_img_size; i < max_img_size; i++)
        {
            suggested_img_size = i;
            if (i * Utils.width > totalAvailableWidth || i * Utils.height > totalAvailableHeight)
                break;
        }
        StartOffset = TopLeft.position;
        GameObject go;
        float half_size = suggested_img_size / 2;
        for (int row = 0; row < Utils.height; row++)
        {
            for (int col = 0; col < Utils.width; col++)
            {
                if(blockPool.TryGetNextObject(StartOffset + new Vector3(col * suggested_img_size + half_size, -row * suggested_img_size - half_size + 1000, 1), Quaternion.identity, out go))
                {
                    BlockBehaviour blk = go.GetComponent<BlockBehaviour>();
                    BlockBehaviour.BlockInfo info;
                    info.GemType = Utils.GetValidGem();
                    info.col = col;
                    info.row = row;
                    info.Id = Utils.GetID(row, col);//row * width + col;
                    info.Size = suggested_img_size;
                    blk.SetupBlock(StartOffset, info);
                    levelData.Add(info.GemType);
                    blocksData.Add(blk);
                }
            }
        }
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
