using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChainMatchController : MonoBehaviour {

    public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;
    HashSet<BlockBehaviour> chain = new HashSet<BlockBehaviour>();
    BlockBehaviour lastBlock = null;

    // This stores the finger that's currently dragging this GameObject
    private Lean.LeanFinger draggingFinger;


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
            if(chain.Add(block))
            {
                lastBlock = block;
                Debug.Log(" BlockAdded " + chain.Count);
            }
        }

    }

    public void OnFingerDown(Lean.LeanFinger finger)
    {
        chain.Clear();
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
        if (finger == draggingFinger)
        {
            // Unset the current finger
            draggingFinger = null;
            lastBlock = null;
        }
        lastBlock = null;
        chain.Clear();
    }
}
