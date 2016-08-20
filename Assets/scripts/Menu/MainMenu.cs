using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EZObjectPools;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public Slider LRSlider;
    public Slider UDSlider;
    public GameObject block;
    public List<int> levelData = new List<int>();
    public List<BlockBehaviour> blocksData = new List<BlockBehaviour>();
    private EZObjectPool blockPool;

    // Use this for initialization
    void Start () {
        blockPool = EZObjectPool.CreateObjectPool(block.gameObject, "Blocks", (int)LRSlider.maxValue * (int)UDSlider.maxValue, true, true, false);
        Invoke("StartLater", 0.5f);
    }
    void StartLater()
    {
        Init();
        Utils.PopulateGrid(ref blockPool, ref blocksData, ref levelData);
    }

    void Init()
    {
        LRSlider.value = Utils.width;
        UDSlider.value = Utils.height;
    }

    public void OnWidthSelect()
    {
        PlayerPrefs.SetInt("LRSlider", (int)LRSlider.value);
        Utils.PopulateGrid(ref blockPool, ref blocksData, ref levelData);
    }

    public void OnHeightSelect()
    {
        PlayerPrefs.SetInt("UDSlider", (int)UDSlider.value);
        Utils.PopulateGrid(ref blockPool, ref blocksData, ref levelData);
    }
    public void OnStartGame()
    {
        SceneManager.LoadScene("game");
    }
}
