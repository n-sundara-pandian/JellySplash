using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChainMatchController : MonoBehaviour {

    public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;
    List<BlockBehaviour> chainList = new List<BlockBehaviour>();
    BlockBehaviour lastBlock = null;
    public LevelLayout levelManager;

    // This stores the finger that's currently dragging this GameObject
    private Lean.LeanFinger draggingFinger;
    public LineRenderer Line;
    List<Vector3> pointsList = new List<Vector3>();

    protected virtual void OnEnable()
    {
        // Hook into the OnFingerDown event
        Lean.LeanTouch.OnFingerDown += OnFingerDown;

        // Hook into the OnFingerUp event
        Lean.LeanTouch.OnFingerUp += OnFingerUp;
    }

    protected virtual void OnDisable()
    {
        // Unhook the OnFingerDown event
        Lean.LeanTouch.OnFingerDown -= OnFingerDown;

        // Unhook the OnFingerUp event
        Lean.LeanTouch.OnFingerUp -= OnFingerUp;
    }

    protected virtual void LateUpdate()
    {
        // If there is an active finger, move this GameObject based on it
        if (draggingFinger != null)
        {
            // Does the main camera exist?
            if (Camera.main != null)
            {
                if (draggingFinger == null)
                    return;
                var ray = draggingFinger.GetRay();
                var hit = default(RaycastHit);
                // Was this finger pressed down on a collider?
                if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true)
                {
                    // Was that collider this one?
                    if (hit.collider.CompareTag("block"))
                    {
                        // Set the current finger to this one
                        AddBlock(hit);
                    }
                }

            }
        }
    }
    void AddBlock(RaycastHit hit )
    {
        // Set the current finger to this one
        GameObject parentObj = hit.transform.parent.gameObject;
        BlockBehaviour block = parentObj.GetComponent<BlockBehaviour>();
        if (block == null)
            return;
        if (lastBlock == null || lastBlock.IsValidNeighbour(block))
        {
            if (chainList.Find(x => x.info.Id == block.info.Id))
            {
                if (block.info.Id == chainList[chainList.Count - 2].info.Id)
                {
                    pointsList.Remove(pointsList[pointsList.Count - 1]);
                    Line.SetVertexCount(pointsList.Count);
                    Line.SetPosition(pointsList.Count - 1, pointsList[pointsList.Count - 1]);
                    lastBlock.SelectBlock(false);
                    chainList.Remove(lastBlock);
                    lastBlock = chainList[chainList.Count - 1];
                    AudioManager.Main.PlayNewSound("deselect");
                }
            }
            else
            {
                pointsList.Add(block.transform.position);
                Line.SetVertexCount(pointsList.Count);
                Line.SetPosition(pointsList.Count - 1, pointsList[pointsList.Count - 1]);
                chainList.Add(block);
                lastBlock = block;
                block.SelectBlock(true);
                AudioManager.Main.PlayNewSound("select");
            }
        }

    }

    public void OnFingerDown(Lean.LeanFinger finger)
    {
        if (draggingFinger != null && finger != draggingFinger)
            return;
        if (levelManager.hsm.GetCurrentState() != LevelLayout.State.Idle)
            return;
        chainList.Clear();
        pointsList.Clear();
        Line.SetWidth(10, 10);
        var ray = finger.GetRay();
        var hit = default(RaycastHit);
        // Was this finger pressed down on a collider?
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true)
        {
            // Was that collider this one?
            if (hit.collider.CompareTag("block"))
            {
                // Set the current finger to this one
                draggingFinger = finger;
                AddBlock(hit);
            }
        }
    }

    public void OnFingerUp(Lean.LeanFinger finger)
    {
        // Was the current finger lifted from the screen?
        if (finger != draggingFinger)
        {
            return;
        }
            // Unset the current finger
            draggingFinger = null;
            lastBlock = null;
        
        lastBlock = null;
        foreach (BlockBehaviour block in chainList)
        {
            block.SelectBlock(false);
        }
        pointsList.Clear();
        Line.SetVertexCount(0);
        if (chainList.Count >= 3)
            levelManager.hsm.Go(LevelLayout.State.Valid_match);
        else
            levelManager.hsm.Go(LevelLayout.State.Invalid_Match);
    }

    public List<BlockBehaviour> GetMatchedChain()
    {
        return chainList;
    }
}
