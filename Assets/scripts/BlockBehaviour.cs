using UnityEngine;
using System.Collections;

public class BlockBehaviour : MonoBehaviour {
    public struct BlockInfo
    {
        public int GemType;
        public int x;
        public int y;
    }
    public Transform block;
    public BlockAnimator Gfx;
    public BlockInfo info;
    public void SetSize(int size,BlockInfo binfo)
    {
        block.localScale = new Vector3(size, size, size);
        info = binfo;
        Gfx.SetGem(info.GemType);
    }
    public bool IsValidNeighbour(BlockBehaviour other)
    {
        BlockInfo binfo = other.info;
        if (info.GemType != binfo.GemType)
            return false;
        int xDiff = Mathf.Abs(info.x - binfo.x);
        int yDiff = Mathf.Abs(info.y - binfo.y);
        if (xDiff == yDiff && xDiff == 0)
        {
            return false;
        }
        if (xDiff <= 1 && yDiff <= 1)
            return true;
        return false;
    }
}
