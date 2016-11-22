using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EZObjectPools;

public class Utils : MonoBehaviour {
    public static int width;
    public static int height;
    public static int highScore;
    public static int longestChain;
    public static int imgSize = 100;
	//public static int current_level = 0;
    public static int GetID(int row, int col)
    {
        return row * width + col;
    }
    public static void GetRowCol(int index, out int row, out int col)
    {
        row = index / width;
        col = index % width;
    }
    public static int GetValidGem()
    {
        return Random.Range(1, 6);
    }
    public static Vector3 RectTransformToWorldPoint(RectTransform transform)
    {
        return Camera.main.ScreenToWorldPoint(transform.TransformPoint(transform.rect.center));
    }

    public static void PopulateGrid(ref EZObjectPool blockPool, ref List<BlockBehaviour> blocksData, ref List<int> levelData)
    {
        Init();
        GameObject temp = GameObject.FindGameObjectWithTag("TopLeft");
        RectTransform TopLeft = temp.GetComponent<RectTransform>();
        temp = GameObject.FindGameObjectWithTag("BottomRight");
        RectTransform BottomRight = temp.GetComponent<RectTransform>();
        temp = GameObject.FindGameObjectWithTag("BackPanel");
        Vector3 StartOffset;
        int min_img_size = 32;
        int max_img_size = 256;
        blockPool.DeActivatePool();
        levelData.Clear();
        blocksData.Clear();
        Vector3 TL = Utils.RectTransformToWorldPoint(TopLeft);
        Vector3 BR = Utils.RectTransformToWorldPoint(BottomRight);
        
        float totalAvailableWidth = Mathf.Abs(TL.x - BR.x);
        float totalAvailableHeight = Mathf.Abs(TL.y - BR.y);
        int suggested_img_size = min_img_size;
        imgSize = min_img_size;
        for (int i = min_img_size; i < max_img_size; i++)
        {
            suggested_img_size = i;
            if (i * Utils.width > totalAvailableWidth || i * Utils.height > totalAvailableHeight)
                break;
        }
        StartOffset = TL;
        float half_size = suggested_img_size / 4;
        float start_y = (totalAvailableHeight - suggested_img_size * (height+ 1)) / 2;
        StartOffset.y -= start_y;
        if (temp != null)
        {
            temp.transform.localScale = new Vector3(suggested_img_size * (width + 0.5f), suggested_img_size * (height + 0.5f), 1);
            temp.transform.position = StartOffset + new Vector3(-half_size, half_size, 2);
        }
        GameObject go;
        for (int row = 0; row < Utils.height; row++)
        {
            for (int col = 0; col < Utils.width; col++)
            {
                if (blockPool.TryGetNextObject(StartOffset + new Vector3(col * suggested_img_size + half_size, -row * suggested_img_size - half_size + 1000, 1), Quaternion.identity, out go))
                {
                    BlockBehaviour blk = go.GetComponent<BlockBehaviour>();
                    BlockBehaviour.BlockInfo info;
                    info.GemType = Utils.GetValidGem();
                    info.col = col;
                    info.row = row;
                    info.Id = Utils.GetID(row, col);
                    info.Size = suggested_img_size;
                    blk.SetupBlock(StartOffset, info);
                    levelData.Add(info.GemType);
                    blocksData.Add(blk);
                }
            }
        }
    }
    static void Init()
    {
		width = 9;//PlayerPrefs.GetInt("LRSlider", 10);
		height = 9;//PlayerPrefs.GetInt("UDSlider", 10);
        highScore = PlayerPrefs.GetInt("highScore", 0);
        longestChain = PlayerPrefs.GetInt("longestChain", 0);

    }
    public static void SavePref(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }
    void Start()
    {
        Init();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
