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
    public static void DetermineGrid(ref EZObjectPool blockPool, ref List<BlockBehaviour> blocksData, ref List<int> levelData)
    {
        Init();
        GameObject temp = GameObject.FindGameObjectWithTag("TopLeft");
        Transform TopLeft = temp.GetComponent<Transform>();
        temp = GameObject.FindGameObjectWithTag("BottomRight");
        Transform BottomRight = temp.GetComponent<Transform>();
        Vector3 StartOffset;
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
