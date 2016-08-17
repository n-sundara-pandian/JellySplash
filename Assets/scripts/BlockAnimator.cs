using UnityEngine;
using System.Collections;

public class BlockAnimator : MonoBehaviour {

    public Animator blockAnimator;
    int GemNumber;
    public void Hide()
    {
        blockAnimator.SetInteger("gem", 0);
    }

    public void SetGem(int GemNo)
    {
        GemNumber = GemNo;
        Debug.Log("Setting Gem " + GemNo);
        blockAnimator.SetInteger("gem", GemNo);
    }
}
