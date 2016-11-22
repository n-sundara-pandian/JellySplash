using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LogoFader : MonoBehaviour {

    public Sprite[] LogoList;
    public Image Logo;
    int index = 0;
	// Use this for initialization
	void Start () {
        Invoke("FadeLogo", 1.0f);
	}
	
	void FadeLogo () {
        index++;
        if (index >= LogoList.Length)
        {
            Application.LoadLevel("mvc");
            return;
        }
        Logo.sprite = LogoList[index];
        Invoke("FadeLogo", 1.0f);
    }
}
