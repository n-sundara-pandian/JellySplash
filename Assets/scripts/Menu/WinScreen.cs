using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class WinScreen : MonoBehaviour {
    public Image img;
    public Text text;
    public Image barImg;
    public Text barText;
    public Image QRImg;
    public Sprite[] ImageList;
    public Sprite[] LocationList;
    public string[] Messages;
    Vector3 OriginalScale;
    enum Screen
    {
        None,
        Congrats,
        HowToGet,
        Redeem,
        WatchVideo,
        Location,
        Terms
    }
    enum Prize
    {
        Starbucks,
        OilOfUlay,
        None
    }
    Screen currentScreen = Screen.None;
    Prize CurrentPrize = Prize.None;
    bool coolOff = true;

    public BarCodeGenerator BarCode;
	// Use this for initialization
	void Start () {
        cool();
        GotoNextState();
	}	
	// Update is called once per frame
	void Update () {
	if (Input.GetMouseButtonDown(0) && coolOff)
        {
            coolOff = false;
            if (!IsInvoking("cool"))
                Invoke("cool", 1.0f);
            GotoNextState();
        }
	}
    void cool()
    {
        coolOff = true;
    }
    void GotoNextState()
    {
        switch(currentScreen)
        {
            case Screen.None:
                {
                    GotoCongrats();
                    break;
                }
            case Screen.Congrats:
                {
                    GotoHowToGet();
                    break;
                }
            case Screen.HowToGet:
                {
                    GotoRedeem();
                    break;
                }
            case Screen.Redeem:
                {
                    GotoWatchVideo();
                    break;
                }
            case Screen.WatchVideo:
                {
                    GotoLocation();
                    break;
                }
            case Screen.Location:
                {
                    GotoTerms();
                    break;
                }
            case Screen.Terms:
                {
                    if (!IsInvoking("LoadMenu"))
                        Invoke("LoadMenu", 1.0f);
                    break;
                }
        }
    }

    void GotoCongrats()
    {
        currentScreen = Screen.Congrats;
        img.sprite = ImageList[0];
        text.text = Messages[0];
    }
    void GotoHowToGet()
    {
        currentScreen = Screen.HowToGet;
        int index = Random.Range(2, 4);
        OriginalScale = img.transform.localScale;
        img.transform.localScale = new Vector3(0,0,0);
        barImg.sprite = Sprite.Create(BarCode.barCodeTexture, new Rect(0, 0, BarCode.barCodeTexture.width, BarCode.barCodeTexture.height), new Vector2(0, 0));
        int prize = Random.Range(0, 2);
        if (prize == 0)
        {
            text.text = Messages[1];
            CurrentPrize = Prize.Starbucks;
            BarCode.SetupQRCode("Starbucks");
        }
        else if (prize == 1)
        {
            text.text = Messages[2];
            CurrentPrize = Prize.OilOfUlay;
            BarCode.SetupQRCode("OilOfUlay");
        }
        else
        {
            CurrentPrize = Prize.None;
        }
        QRImg.sprite = Sprite.Create(BarCode.qrCodeTexture, new Rect(0, 0, BarCode.qrCodeTexture.width, BarCode.qrCodeTexture.height), new Vector2(0, 0));
        barText.text = BarCode.Lastresult;
    }
    void GotoRedeem()
    {
        currentScreen = Screen.Redeem;
     /*   if (CurrentPrize == Prize.Starbucks)
        {
            text.text = Messages[3];
        }
        else if (CurrentPrize == Prize.OilOfUlay)
        {
            text.text = Messages[4];
        }*/
    }
    void GotoTerms()
    {
        currentScreen = Screen.Terms;
        img.transform.localScale = Vector3.zero;
        HideBarCodeImages();
        text.text = Messages[5];
    }
    void GotoLocation()
    {        
        currentScreen = Screen.Location;
        img.transform.localScale = OriginalScale;
        if (CurrentPrize == Prize.Starbucks)
        {
            img.sprite = LocationList[0];
        }
        else if (CurrentPrize == Prize.OilOfUlay)
        {
            img.sprite = LocationList[1];
        }
        text.text = Messages[5];
    }
    void GotoWatchVideo()
    {
        currentScreen = Screen.WatchVideo;
        if (CurrentPrize == Prize.Starbucks)
        {
            Handheld.PlayFullScreenMovie("StarbucksAd.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
            text.text = Messages[3];

        }
        else if (CurrentPrize == Prize.OilOfUlay)
        {
            Handheld.PlayFullScreenMovie("OilOfUlay.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
            text.text = Messages[4];

        }
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("menu");
    }
    void HideBarCodeImages()
    {
        barImg.transform.localScale = Vector3.zero;
        barText.transform.localScale = Vector3.zero;
        QRImg.transform.localScale = Vector3.zero;
    }
}
