using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EZObjectPools;

public class Utils : MonoBehaviour {
    public static int width;
    public static int height;
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
    public static void PopulateGrid(ref EZObjectPool blockPool, ref List<BlockBehaviour> blocksData, ref List<int> levelData)
    {
        Init();
        GameObject temp = GameObject.FindGameObjectWithTag("TopLeft");
        Transform TopLeft = temp.GetComponent<Transform>();
        temp = GameObject.FindGameObjectWithTag("BottomRight");
        Transform BottomRight = temp.GetComponent<Transform>();
        temp = GameObject.FindGameObjectWithTag("BackPanel");
        Vector3 StartOffset;
        int min_img_size = 32;
        int max_img_size = 256;
        blockPool.DeActivatePool();
        levelData.Clear();
        blocksData.Clear();
        float totalAvailableWidth = Mathf.Abs(TopLeft.position.x - BottomRight.position.x);
        float totalAvailableHeight = Mathf.Abs(TopLeft.position.y - BottomRight.position.y);
        int suggested_img_size = min_img_size;
        for (int i = min_img_size; i < max_img_size; i++)
        {
            suggested_img_size = i;
            if (i * Utils.width > totalAvailableWidth || i * Utils.height > totalAvailableHeight)
                break;
        }
        StartOffset = TopLeft.position;
        float half_size = suggested_img_size / 2;
        if (temp != null)
        {
            temp.transform.localScale = new Vector3(suggested_img_size * (width + 1), suggested_img_size * (height + 1), 1);
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
        width = PlayerPrefs.GetInt("LRSlider", 10);
        height = PlayerPrefs.GetInt("UDSlider", 10);

    }
    void Start()
    {
        Init();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
