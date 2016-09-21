using UnityEngine;
using System.Collections;
using DG.Tweening;
using thelab.mvc;
public class BlockBehaviour :  View<Game> {
    public struct BlockInfo
    {
        public int GemType;
        public int col;
        public int row;
        public int Id;
        public int Size;
    }   
    public Transform block;
    public BlockAnimator Gfx;
    public BlockInfo info;
    public TextMesh text;
    public float shakeDegree = 100.0f;
    float fallTime = 0.25f;
    Vector3 offset;
    public int moveDown = 0;
    public void GetIntoPosition()
    {
        block.transform.DOMove(offset + new Vector3(info.col * info.Size + (info.Size / 2), -info.row * info.Size - (info.Size / 2), 1), fallTime);
    }
    public void HideItem()
    {
        SetGem(0);
    }

    public void MoveDown()
    {
        if (moveDown <= 0)
        {
            moveDown = 0;
            return;
        }
        block.transform.DOMove(offset + new Vector3(info.col * info.Size + (info.Size / 2), -(info.row + moveDown) * info.Size - (info.Size / 2), 1), fallTime);
        moveDown = 0;
    }
    public void SetMoveDown(int inc)
    {
        moveDown += inc;
    }
    public void SetGem(int gemno)
    {
        info.GemType = gemno;
		if (gemno >= 0)
			Gfx.SetGem(app.view.GemList[info.GemType], app.view.HighlightList[info.GemType]);
		else
			Gfx.SetGem(app.view.GemList[0], app.view.GemList[0]);
        text.text = info.Id.ToString();
    }

    public void UpdatePosition(int yoffset = 0)
    {
        block.transform.position = offset + new Vector3(info.col * info.Size + (info.Size / 2), -(info.row + yoffset) * info.Size - (info.Size / 2), 1);
    }

    public void SetupBlock(Vector3 off,BlockInfo binfo)
    {
        offset = off;
        info = binfo;
        block.localScale = new Vector3(binfo.Size, binfo.Size, binfo.Size);
        SetGem(info.GemType);
        GetIntoPosition();
    }
	public void SetupTile(Vector3 off,BlockInfo binfo)
	{
		offset = off;
		info = binfo;
		block.localScale = new Vector3(binfo.Size, binfo.Size, binfo.Size);
		GetIntoPosition();
	}
    public void SelectBlock(bool flag)
    {
        if (flag)
        {
            block.transform.DORotate(Vector3.forward * -shakeDegree, 0.1f);
            block.transform.DORotate(Vector3.forward * shakeDegree, 0.25f).SetLoops(-1, LoopType.Yoyo).SetDelay(0.1f);
			Gfx.HighLight ();
        }
        else
        {
            block.transform.DOKill(true);
            block.transform.DORotate(Vector3.zero, 0.1f);
			Gfx.Normal ();
        }
    }
    public bool IsValidNeighbour(BlockBehaviour other)
    {
        BlockInfo binfo = other.info;
        if (info.GemType != binfo.GemType)
            return false;
		if (info.GemType == 0)
			return false;
        int xDiff = Mathf.Abs(info.col - binfo.col);
        int yDiff = Mathf.Abs(info.row - binfo.row);
        if (xDiff == yDiff && xDiff == 0)
        {
            return false;
        }
        if (xDiff <= 1 && yDiff <= 1)
            return true;
        return false;
    }
	public bool IsValidGem()
	{
		return (info.GemType > 0);
	}
}
