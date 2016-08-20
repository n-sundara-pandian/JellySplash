using UnityEngine;
using System.Collections;

public class BlockAnimator : MonoBehaviour {

    public Animator blockAnimator;
    public void Hide()
    {
        blockAnimator.SetInteger("gem", 0);
    }

    public void SetGem(int GemNo)
    {
        blockAnimator.SetInteger("gem", 0);
        blockAnimator.SetInteger("gem", GemNo);
    }
}
