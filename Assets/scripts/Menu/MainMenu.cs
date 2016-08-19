using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour {

    public Slider LRSlider;
    public Slider UDSlider;
    // Use this for initialization
    void Start () {
        LRSlider.value = Utils.width;
        UDSlider.value = Utils.height;
    }

    public void OnWidthSelect()
    {
        PlayerPrefs.SetInt("LRSlider", (int)LRSlider.value);
    }

    public void OnHeightSelect()
    {
        PlayerPrefs.SetInt("UDSlider", (int)UDSlider.value);
    }
}
