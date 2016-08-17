using UnityEngine;
using System.Collections;
using EZObjectPools;
public class LevelLayout : MonoBehaviour {

    public int width = 1;
    public int height = 1;
    public Transform TopLeft;
    public Transform BottomRight;

    Vector3 StartPoint;
    Vector2 GridSize;

    int min_img_size = 32;
    int max_img_size = 256;
    float mid_x;

    int suggested_img_size;

    public EZObjectPool blockPool;
    public GameObject block;
	// Use this for initialization
	void Start () {
        blockPool = EZObjectPool.CreateObjectPool(block.gameObject, "Blocks", width * height * 2, true, true, false);
        DetermineGrid();
    }

    public void DetermineGrid()
    {
        blockPool.DeActivatePool();
        float totalAvailableWidth = Mathf.Abs(TopLeft.position.x - BottomRight.position.x);
        float totalAvailableHeight = Mathf.Abs(TopLeft.position.y - BottomRight.position.y);
        mid_x = (TopLeft.position.x + totalAvailableWidth) / 2.0f;
        suggested_img_size = min_img_size;
        for (int i = min_img_size; i < max_img_size; i++)
        {
            suggested_img_size = i;
            if (i * width > totalAvailableWidth || i * height > totalAvailableHeight)
                break;
        }
        StartPoint = TopLeft.position;
        Debug.Log(suggested_img_size);
        GameObject go;
        float half_size = suggested_img_size / 2;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if(blockPool.TryGetNextObject(StartPoint + new Vector3(x * suggested_img_size + half_size, -y * suggested_img_size - half_size, 1), Quaternion.identity, out go))
                {
                    BlockBehaviour blk = go.GetComponent<BlockBehaviour>();
                    BlockBehaviour.BlockInfo info;
                    info.GemType = Random.Range(1, 6);
                    info.x = x;
                    info.y = y;
                    blk.SetSize(suggested_img_size, info);
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void CreateLevel()
    {
        if (width <= 0)
            return;
        if (height <= 0)
            return;
    }
}
