using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    // Use this for initialization
    public void Start()
    {
    }

    public Texture logo;

    public void Update()
    {

    }

    public void OnGUI()
    {



        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.fontSize = 30;
        var width = Screen.width;
        var height = Screen.height;
        var logoWidth = width * 3 / 4;
        var logoHeight = height / 4;
        GUI.Box(new Rect(width / 2 - logoWidth / 2, height / 4, logoWidth, logoHeight), logo);

        var buttonWidth = width / 7;
        var buttonHeight = height / 15;
        Debug.Log(style);
        var play = GUI.Button(new Rect(width / 2 - buttonWidth / 2, height * 5 / 8, buttonWidth, buttonHeight), "Scan QR Code", style);
        if (play)
        {
            SceneManager.LoadScene("QRCode");
            return;

        }
        var tryMe = GUI.Button(new Rect(width / 2 - buttonWidth / 2, height * 6 / 8, buttonWidth, buttonHeight), "Try Me", style);
        if (tryMe)
        {
            SceneManager.LoadScene("LoadData");
            return;
        }

        var url = GUI.Button(new Rect(width / 2 - buttonWidth / 2, height * 7 / 8, buttonWidth, buttonHeight), "Clipboard",
            style);
        if (url)
        {
            StaticReplayScript.URL = UniClipboard.GetText();
            SceneManager.LoadScene("LoadData");
        }
    }
}
