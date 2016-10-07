using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;
using Vectrosity;

public class ChainMatchController : Controller<Game> {

    //public LevelLayout levelManager;
    public List<BlockBehaviour> chainList = new List<BlockBehaviour>();
	private List<Vector3> pointsList3d = new List<Vector3>();
	VectorLine vLine = null;// = new VectorLine ("3dLine", pointsList, 1.0f);
    private LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;
    private BlockBehaviour lastBlock = null;
    private Lean.LeanFinger draggingFinger;
	void Start()
	{
		vLine = new VectorLine ("3dLine", pointsList3d, 5.0f, LineType.Continuous, Joins.Fill);
		vLine.color = Color.grey;
	}

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
		if (!block.IsValidGem ())
			return;
        if (lastBlock == null || lastBlock.IsValidNeighbour(block))
        {
            if (chainList.Find(x => x.info.Id == block.info.Id))
            {
                if (block.info.Id == chainList[chainList.Count - 2].info.Id)
                {
					pointsList3d.Remove(pointsList3d[pointsList3d.Count - 1]);
                    lastBlock.SelectBlock(false);
                    chainList.Remove(lastBlock);
                    lastBlock = chainList[chainList.Count - 1];
                    AudioManager.Main.PlayNewSound("deselect");
                }
            }
            else
            {
				pointsList3d.Add(block.transform.position);
                chainList.Add(block);
                lastBlock = block;
                block.SelectBlock(true);
                AudioManager.Main.PlayNewSound("select");
            }
        }
		app.view.SetValue ("curscore", app.model.GetScoreForBlockCount (chainList.Count));
		vLine.Draw3DAuto ();
    }
    public void OnFingerDown(Lean.LeanFinger finger)
    {
        if (draggingFinger != null && finger != draggingFinger)
            return;
        if (app.controller.hsm.GetCurrentState() != HSM.State.Idle)
            return;
        chainList.Clear();
        pointsList3d.Clear();
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
		if (app.controller.hsm.GetCurrentState() != HSM.State.Idle)
			return;
            // Unset the current finger
      
        foreach (BlockBehaviour block in chainList)
        {
            block.SelectBlock(false);
        }
		pointsList3d.Clear ();
		if (chainList.Count == 1) {
			app.controller.hsm.Go(HSM.State.FloodFill);

		}
        else if (chainList.Count >= 3)
            app.controller.hsm.Go(HSM.State.Valid_match);
        else
        {
            app.controller.hsm.Go(HSM.State.Invalid_Match);
			if (chainList.Count > 0)
            AudioManager.Main.PlayNewSound("deselect");
        }
		app.view.SetValue ("curscore", app.model.GetScoreForBlockCount (0));
		draggingFinger = null;
		lastBlock = null;
    }

    public List<BlockBehaviour> GetMatchedChain()
    {
        return chainList;
    }
	public int GetLastBlockID()
	{
		return chainList[chainList.Count - 1].info.Id;
	}
	public void Reset()
	{
		lastBlock = null;
		chainList.Clear ();
	}
}
