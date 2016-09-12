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
    public Text StatusText;
    int currentText = 0;
    private EZObjectPool blockPool;
    List<string> statusMsgs = new List<string>();
    // Use this for initialization
    void Start () {
        blockPool = EZObjectPool.CreateObjectPool(block.gameObject, "Blocks", (int)LRSlider.maxValue * (int)UDSlider.maxValue, true, true, false);
        Invoke("StartLater", 0.5f);
    }
    void Init()
    {
        LRSlider.value = Utils.width;
        UDSlider.value = Utils.height;
    }
    void StartLater()
    {
        Init();
        Utils.PopulateGrid(ref blockPool, ref blocksData, ref levelData);
        statusMsgs.Add("Adjust the sliders to your desired grid size and tap on Play Game button to Start the Game");
        if (Utils.longestChain != 0)
        {
            string msg = string.Format("Your longest chain is {0} \n Your high score is {1} ", Utils.longestChain, Utils.highScore);
            statusMsgs.Add(msg);
            Invoke("ChangeStatusText", Random.Range(1, 3));
        }        
    }
    void ChangeStatusText()
    {
        StatusText.text = statusMsgs[currentText];
        currentText++;
        currentText = currentText % statusMsgs.Count;
        Invoke("ChangeStatusText", Random.Range(5, 12));
    }

    public void OnWidthSelect()
    {
        Utils.SavePref("LRSlider", (int)LRSlider.value);
        Utils.PopulateGrid(ref blockPool, ref blocksData, ref levelData);
    }

    public void OnHeightSelect()
    {
        Utils.SavePref("UDSlider", (int)UDSlider.value);
        Utils.PopulateGrid(ref blockPool, ref blocksData, ref levelData);
    }
    public void OnStartGame()
    {
        SceneManager.LoadScene("mvc");
    }
}
