using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EZObjectPools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using thelab.mvc;

public class GameView : View<Game> {
    public GameObject block;
	public GameObject tile;
    public GameObject particle;
    public Text scoreText;
	public Text curMoveScoreText;
    public Text movesText;
    public Image resultImage;
    public Text resultText;

    private List<BlockBehaviour> blocksData = new List<BlockBehaviour>();
    private Sound bgMusic;
    private EZObjectPool blockPool;
    private EZObjectPool particlePool;
	private EZObjectPool backTilePool;
	private EZObjectPool frontTilePool;   

	private Vector3 backTileOffset = new Vector3(0,0,1);
	private Vector3 frontTileOffset = new Vector3 (0, 0, -1);
	public bool warned = false;
    public bool congradulated = false;
    bool msgBoxBusy = false;

	public Sprite[] GemList;
	public Sprite[] HighlightList;

    void Start()
    {
        resultImage.transform.DOMoveX(2000, 0.1f);
    }
    public void InitBoardGfx(List<int> levelData)
    {
        blockPool = EZObjectPool.CreateObjectPool(block.gameObject, "Blocks", Utils.width * Utils.height * 2, true, true, false);
        blockPool.DeActivatePool();
		backTilePool = EZObjectPool.CreateObjectPool(tile.gameObject, "backTiles", Utils.width * Utils.height, true, true, false);
		backTilePool.DeActivatePool();
		frontTilePool = EZObjectPool.CreateObjectPool(block.gameObject, "frontTile", Utils.width * Utils.height, true, true, false);
		frontTilePool.DeActivatePool();
        particlePool = EZObjectPool.CreateObjectPool(particle.gameObject, "Particles", (Utils.width * Utils.height) / 2, true, true, false);
        particlePool.DeActivatePool();
        GameObject temp = GameObject.FindGameObjectWithTag("TopLeft");
        RectTransform TopLeft = temp.GetComponent<RectTransform>();
        temp = GameObject.FindGameObjectWithTag("BottomRight");
        RectTransform BottomRight = temp.GetComponent<RectTransform>();
        temp = GameObject.FindGameObjectWithTag("BackPanel");
        Vector3 StartOffset;
        int min_img_size = 32;
        int max_img_size = 256;
        blocksData.Clear();
        Vector3 TL = Utils.RectTransformToWorldPoint(TopLeft);
        Vector3 BR = Utils.RectTransformToWorldPoint(BottomRight);

        float totalAvailableWidth = Mathf.Abs(TL.x - BR.x);
        float totalAvailableHeight = Mathf.Abs(TL.y - BR.y);
        int suggested_img_size = min_img_size;
        for (int i = min_img_size; i < max_img_size; i++)
        {
            suggested_img_size = i;
            if (i * Utils.width > totalAvailableWidth || i * Utils.height > totalAvailableHeight)
                break;
        }
        StartOffset = TL;
        float half_size = suggested_img_size / 4;
        float start_y = (totalAvailableHeight - suggested_img_size * (Utils.height + 1)) / 2;
        StartOffset.y -= start_y;
        if (temp != null)
        {
       //     Debug.Log("Setting Panel Size");
      //      temp.transform.localScale = new Vector3(suggested_img_size * (Utils.width + 0.5f), suggested_img_size * (Utils.height + 0.5f), 1);
       //     temp.transform.position = StartOffset + new Vector3(-half_size, half_size, 2);
        }
        GameObject go;
        for (int row = 0; row < Utils.height; row++)
        {
            for (int col = 0; col < Utils.width; col++)
            {
				Vector3 pos = StartOffset + new Vector3 (col * suggested_img_size + half_size, -row * suggested_img_size - half_size , 1);
                if (blockPool.TryGetNextObject(pos, Quaternion.identity, out go))
                {
                    BlockBehaviour blk = go.GetComponent<BlockBehaviour>();
                    BlockBehaviour.BlockInfo info;
                    info.GemType = levelData[Utils.GetID(row, col)];
                    info.col = col;
                    info.row = row;
                    info.Id = Utils.GetID(row, col);
                    info.Size = suggested_img_size;
                    blk.SetupBlock(StartOffset, info);
                    blocksData.Add(blk);
					if (info.GemType > 0) 
					{
						if (backTilePool.TryGetNextObject (pos + backTileOffset, Quaternion.identity, out go))
						{
							BlockBehaviour tle = go.GetComponent<BlockBehaviour>();
							BlockBehaviour.BlockInfo tileInfo;
							tileInfo.GemType = levelData[Utils.GetID(row, col)];
							tileInfo.col = col;
							tileInfo.row = row;
							tileInfo.Id = Utils.GetID(row, col);
							tileInfo.Size = suggested_img_size;
							tle.SetupTile(StartOffset, tileInfo);
						}

					}
                }
            }
        }
    }
    void ShowFx(Vector3 pos, int gemno)
    {
        GameObject go;
        if (particlePool.TryGetNextObject(pos, Quaternion.identity, out go))
        {
            ShockFlame fx = go.GetComponent<ShockFlame>();
            if (fx != null)
            {
                fx.SetColor(gemno);
            }
        }
    }
    public void ShowMessage(string msg, float delay)
    {
        StartCoroutine(SetMessage(msg, delay));
    }
    IEnumerator SetMessage(string msg, float delay)
    {
        while(msgBoxBusy)
            yield return new WaitForSeconds(0.1f);
        if (msgBoxBusy)
            yield return new WaitForSeconds(1.0f);
        msgBoxBusy = true;
        resultText.text = msg;
        Vector3 pos = resultImage.transform.position;
        pos.x = 2000;
        resultImage.transform.position = pos;
        yield return new WaitForSeconds(0.1f);
        resultImage.transform.DOMoveX(0, 0.5f);
        yield return new WaitForSeconds(delay);
        resultImage.transform.DOMoveX(-2000, 0.25f);
        yield return new WaitForSeconds(0.25f);
        msgBoxBusy = false;
    }

    IEnumerator PlayEndGame(int moves, int score)
    {
        string result = string.Format("Good Job ! You have scored {0} points in {1} moves", score, moves);
        StartCoroutine(SetMessage(result, 5.0f));
        yield return new WaitForSeconds(5.5f);
        Notify("view.OnEndGameComplete");
    }
    public void HideItem(BlockBehaviour blk, float delay)
    {
        StartCoroutine(Disappear(blk, delay));
    }
    IEnumerator Disappear(BlockBehaviour blk, float delay)
    {
        yield return new WaitForSeconds(delay);
		AudioManager.Main.PlayNewSound("dispear");
        ShowFx(blk.transform.position, blk.info.GemType);
        AudioManager.Main.PlayNewSound("dispear");
        blocksData[blk.info.Id].SetGem(0);
        blk.HideItem();
    }
    public void FallDown(List<int> levelData, List<BlockBehaviour> chainList, float delay)
    {
        StartCoroutine(FallDownCR(levelData, chainList, delay));

    }
    IEnumerator FallDownCR(List<int> levelData, List<BlockBehaviour> chainList, float delay)
    {
        foreach (BlockBehaviour blk in chainList)
        {
            int id = blk.info.Id;
            for (int i = id; i >= 0; i -= Utils.width)
            {
                if (levelData[i] != 0)
                {
                    blocksData[i].SetMoveDown(1);
                }
            }
        }
        yield return new WaitForSeconds(delay);
		AudioManager.Main.PlayNewSound("falldown");
        foreach (BlockBehaviour blk in blocksData)
        {
            blk.MoveDown();
        }
        yield return new WaitForSeconds(0.25f);
        SyncBoard(levelData);
        for (int i = 0; i < blocksData.Count; i++)
        {
            if (levelData[i] == 0)
            {
                blocksData[i].UpdatePosition(-10);
            }
        }
        Notify("view.fill_items");
    }
    public void FillItems(List<int> levelData)
    {
        for (int i = 0; i < blocksData.Count; i++)
        {
            blocksData[i].SetGem(levelData[i]);
            blocksData[i].GetIntoPosition();
        }
    }
    public void SyncBoard(List<int> levelData)
    {
        for (int i = blocksData.Count - 1; i >= 0; --i)
        {
            blocksData[i].SetGem(levelData[i]);
            blocksData[i].UpdatePosition();
        }
    }

    public void SetValue(string key, int val)
    {
		if (key == "score")
			scoreText.text = val.ToString ();
		else if (key == "curscore") {
			if (val >= 300)
				curMoveScoreText.text = val.ToString ();
			else
				curMoveScoreText.text = "";
		}
        else if (key == "move")
            movesText.text = val.ToString();
    }
    public void WinGame(int moves, int score)
    {
        StartCoroutine(PlayEndGame(moves, score));
    }
    public void LoseGame()
    {
        ShowMessage(" Oh No! Failed To Acheive Target. Please Try Again", 3.0f);
        Invoke("LoadMenu", 3.0f);
    }
	public BlockBehaviour GetBlockForID(int id)
	{
		for (int i = 0; id < blocksData.Count; i++) {
			if (blocksData [i].info.Id == id)
				return blocksData [i];
		}
		return null;	
	}
    void LoadMenu()
    {
        SceneManager.LoadScene("menu1");
    }
}
