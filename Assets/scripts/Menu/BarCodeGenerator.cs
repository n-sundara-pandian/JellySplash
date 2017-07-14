
using UnityEngine;
using System.Collections;
using ZXing;
using ZXing.QrCode;
using UnityEngine.UI;
using System.IO;

public class BarCodeGenerator : MonoBehaviour
{
    public Texture2D barCodeTexture;
    public Texture2D qrCodeTexture;
    public Texture2D inputTexture;
    public string Lastresult;
    public Renderer webCam;
    WebCamTexture wct;
    //public RawImage ima; 

    void Start()
    {
        barCodeTexture = new Texture2D(256, 64);
        qrCodeTexture = new Texture2D(256, 256);
        Lastresult = GetGUID();

        var textForEncoding = Lastresult;
        var color321 = BarEncode(BarcodeFormat.QR_CODE, "Get an free coffee from starbucks", qrCodeTexture.width, qrCodeTexture.height);
        qrCodeTexture.SetPixels32(color321);
        qrCodeTexture.Apply();
        if (textForEncoding != null)
        {
            var color32 = BarEncode(BarcodeFormat.CODE_128, textForEncoding, barCodeTexture.width, barCodeTexture.height);
            barCodeTexture.SetPixels32(color32);
            barCodeTexture.Apply();
        }
        StartWebCam();
    }

    public void SetupQRCode(string textForEncoding)
    {

    }

    // for generate qrcode
    private Color32[] QREncode(BarcodeFormat fmt, string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = fmt,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }
    public void BarDecode()
    {
        Decode(inputTexture.GetPixels32(), inputTexture.width, inputTexture.height);
    }

    void Decode(Color32[] bytes, int width, int height)
    {
        
        BarcodeReader reader = new BarcodeReader { AutoRotate = true};
        reader.Options.TryHarder = true;
        // get texture Color32 array
        // detect and decode the barcode inside the Color32 array
        var result = reader.Decode(bytes, width, height);
        // do something with the result
        if (result != null)
        {
            Debug.Log(result.BarcodeFormat.ToString());
            Debug.Log(result.Text);
        }
    }

    IEnumerator TakePhoto()
    {

        // NOTE - you almost certainly have to do this here:

        yield return new WaitForEndOfFrame();

        // it's a rare case where the Unity doco is pretty clear,
        // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
        // be sure to scroll down to the SECOND long example on that doco page 

        Texture2D photo = new Texture2D(wct.width, wct.height);
        photo.SetPixels(wct.GetPixels());
        photo.Apply();
        //    photo = TextureFilter.Convolution(photo, TextureFilter.SHARPEN_KERNEL, 2);
      //  photo = TextureFilter.Grayscale(photo);
        //Encode to a PNG
        byte[] bytes = photo.EncodeToPNG();
        //Write out the PNG. Of course you have to substitute your_path for something sensible
        var filepath = Path.Combine(Application.persistentDataPath, "photo.png");
        File.WriteAllBytes(filepath, bytes);
        Debug.Log(filepath);
        Decode(photo.GetPixels32(), photo.width, photo.height);
    }

    public void StartWebCam()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        string deviceName = devices[1].name;
        wct = new WebCamTexture(deviceName, 1024, 1024, 12);
        webCam.material.mainTexture = wct;        
        wct.Play();
    }
    private Color32[] BarEncode(BarcodeFormat fmt,string textForEncoding, int width, int height)
    {        
        var writer = new BarcodeWriter
        {
            Format = fmt,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }
    string GetGUID()
    {
        string str = "";
        byte[] buffer = new byte[8];
        for (int b = 0; b < 8; ++b)
        {
            buffer[b] = (byte)Random.Range(0, 255);
            str += buffer[b].ToString("X").PadLeft(2, '0');
        }
        return str;
    }
    public void CaptureAndDecode()
    {
        StartCoroutine(TakePhoto());
    }

    /*
    void OnGUI()
    {
        GUI.DrawTexture(new Rect(100, 100, 256, 64), encoded);
    }*/
}
