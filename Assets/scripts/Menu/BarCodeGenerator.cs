
using UnityEngine;  
using System.Collections;  
using ZXing; 
using ZXing.QrCode;  
using UnityEngine.UI;  

public class BarCodeGenerator : MonoBehaviour
{
    public Texture2D barCodeTexture;
    public Texture2D qrCodeTexture;
    public string Lastresult;

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
    /*
    void OnGUI()
    {
        GUI.DrawTexture(new Rect(100, 100, 256, 64), encoded);
    }*/
}
