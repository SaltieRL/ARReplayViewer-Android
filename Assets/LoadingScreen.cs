using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {

    private string dataMsg;
    private string protoMsg;
    private string replayID ;

    public string SceneToLoad;
    public void Start()
    {
        var url = StaticReplayScript.URL;
        StaticReplayScript.proto = null;
        StaticReplayScript.gameData = null;
        Debug.Log(url);
        if (url.StartsWith("https://calculated.gg"))
        {
            string[] sections = url.Split('/');
            replayID = sections[sections.Length - 2];
            Debug.Log(replayID);
            WWW req = new WWW(url);
            StartCoroutine("GetData", req);
        }

    }

    IEnumerator GetProtoData(WWW protoWWW)
    {
// protoMsg = "Loading proto.";
        Debug.Log("Loading proto");
        yield return protoWWW;
        Debug.Log("Loaded proto.");
        if (protoWWW.error == null)
        {
            string text = protoWWW.text;
            Debug.Log(text.Substring(0, 100));
            Proto r = JsonConvert.DeserializeObject<Proto>(text);
            StaticReplayScript.proto = r;
            if (StaticReplayScript.gameData != null && SceneToLoad != "" && SceneToLoad != null)
            {
                SceneManager.LoadScene(SceneToLoad);
            }
        }
        else
        {
            Debug.Log(protoWWW.error);
            protoMsg = string.Format("(P) Error: {0}", protoWWW.error);
        }

    }
    IEnumerator GetData(WWW req)
    {

        yield return req;
        if (req.error == null)
        {
            string text = req.text;
            Debug.Log(text.Substring(0, 100));
            RootObject r = JsonConvert.DeserializeObject<RootObject>(text);
            Debug.Log(r.id);
            Debug.Log(r.colors[0]);
            Debug.Log(r.frames[0][0]);
            Debug.Log(r.ball[0][0]);
            Debug.Log(r.players[0][0][0]);
            StaticReplayScript.gameData = r;
            if (StaticReplayScript.proto == null)
            {
                //dynamic dyn = JsonUtility.FromJson(res);
                var protoURL = "https://calculated.gg/api/v1/replay/" + replayID + "?key=1";
                Debug.Log(protoURL);
                WWW protoWWW = new WWW(protoURL);
                StartCoroutine(GetProtoData(protoWWW));
                //SceneManager.LoadScene("HelloAR");
            }
        }
        else
        {
            Debug.Log(req.error);
            dataMsg = string.Format("(D) Error: {0}", req.error);
        }
    }

    void OnGUI()
    {
        var width = Screen.width;
        var height = Screen.height;
        var textWidth = width/3;
        var textHeight = height/6;

        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 30;
        style.normal.textColor = Color.white;

        if (StaticReplayScript.gameData == null)
        {
            
            GUI.Label(new Rect(width / 2 - textWidth / 2, height/3 - textHeight/2, textWidth, textHeight), "Loading game data...", style);
        }
        else if (StaticReplayScript.proto == null)
        {

            GUI.Label(new Rect(width / 2 - textWidth / 2, height / 3 - textHeight / 2, textWidth, textHeight), "Loading proto...", style);
        }

        if (protoMsg != null)
        {
            GUI.Label(new Rect(width / 2 - textWidth / 2, height /3 - textHeight/2, textWidth, textHeight), protoMsg);
        }
    }
}
