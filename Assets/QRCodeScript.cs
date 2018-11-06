using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZXing;
using ZXing.QrCode;

public class QRCodeScript : MonoBehaviour {

    private WebCamTexture camTexture;
    private Rect screenRect;
    private int frameCounter = 0;
    private string text = "";
    void Start()
    {
        screenRect = new Rect(0, 0, Screen.width, Screen.height);
        camTexture = new WebCamTexture();
        camTexture.requestedHeight = Screen.height;
        camTexture.requestedWidth = Screen.width;
        Debug.Log(camTexture);
        if (camTexture != null)
        {
            camTexture.Play();
        }
    }

    void OnGUI()
    {
        frameCounter += 1;
        // drawing the camera on screen
        GUI.DrawTexture(screenRect, camTexture, ScaleMode.ScaleToFit);
        if (text != "")
        {
            GUI.Label(screenRect, text);
        }
        if (frameCounter == 30)
        {
            frameCounter = 0;
            // do the reading — you might want to attempt to read less often than you draw on the screen for performance sake
            try
            {
                IBarcodeReader barcodeReader = new BarcodeReader();
                // decode the current frame
                var result = barcodeReader.Decode(camTexture.GetPixels32(),
                    camTexture.width, camTexture.height);
                if (result != null)
                {
                    Debug.Log("DECODED TEXT FROM QR: " + result.Text);
                    text = result.Text;
                    StaticReplayScript.URL = result.Text;
                    SceneManager.LoadScene("HelloAR");
                    //Handles.Label(gameObject.transform.position, result.Text);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
    }
}
