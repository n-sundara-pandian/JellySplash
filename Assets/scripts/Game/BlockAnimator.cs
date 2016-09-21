using UnityEngine;
using System.Collections;

public class BlockAnimator : MonoBehaviour {

    public Animator blockAnimator;
	public SpriteRenderer sprite;
	Sprite normalTexture;
	Sprite highlightTexture;

	public void SetGem(Sprite texture, Sprite high)
    {
		normalTexture = texture;
		highlightTexture = high;
		Normal ();
    }
	public void HighLight()
	{
		sprite.sprite = highlightTexture;
	}
	public void Normal()
	{
		sprite.sprite = normalTexture;
	}
}
