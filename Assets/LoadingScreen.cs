using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class LoadingScreen : MonoBehaviour {

    private string dataMsg;
    private string protoMsg;
    private string replayID;
    //public GameObject loadingCircle;
    //private GameObject circleInstance;

    public string SceneToLoad;
    public void Start()
    {
        var width = Screen.width;
        var height = Screen.height;
        var url = StaticReplayScript.URL;
        StaticReplayScript.proto = null;
        StaticReplayScript.gameData = null;
        Debug.Log(url);
        if (url.StartsWith("https://calculated.gg"))
        {
            string[] sections = url.Split('/');
            if (url.EndsWith("positions"))
            {
                replayID = sections[sections.Length - 2];
            }
            else
            {
                replayID = sections[sections.Length - 1];
            }
            url = string.Format("https://calculated.gg/api/replay/{0}/positions", replayID);
            Debug.Log(replayID);
            WWW req = new WWW(url);
            StartCoroutine("GetData", req);
        }
        //if (loadingCircle != null)
        //{
        //circleInstance = Instantiate(loadingCircle);
        //RectTransform transform = circleInstance.GetComponent<RectTransform>();
        //transform.position = new Vector3(0, 0, 0);
        //}
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
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) =>
                {
                    Debug.Log(args);
                }
            };
            ITraceWriter traceWriter = new MemoryTraceWriter() { LevelFilter = TraceLevel.Verbose };
            Proto r = JsonConvert.DeserializeObject<Proto>(text, settings);//new JsonSerializerSettings() { TraceWriter = traceWriter });
            Debug.Log(traceWriter);
            Debug.Log("Object is null: " + (r == null));
            
            StaticReplayScript.proto = r;
            if (StaticReplayScript.gameData != null && !string.IsNullOrEmpty(SceneToLoad))
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
        style.fontSize = 60;
        style.normal.textColor = Color.white;

        if (StaticReplayScript.gameData == null)
        {
            
            GUI.Label(new Rect(width / 2 - textWidth / 2, height/3 - textHeight/2, textWidth, textHeight), "Loading game data...", style);
        }
        else if (StaticReplayScript.proto == null)
        {

            GUI.Label(new Rect(width / 2 - textWidth / 2, height / 3 - textHeight / 2, textWidth, textHeight), "Loading game metadata...", style);
        }
        
        style.fontSize = 40;
        if (StaticReplayScript.gameData == null || StaticReplayScript.proto == null)
        {
            GUI.Label(new Rect(width * 1/4f, height*18/20f, width/2f, height*2/20f),
                "Point towards a surface and click on the white grid when it appears.", style);
        }

        if (protoMsg != null)
        {
            GUI.Label(new Rect(width / 2 - textWidth / 2, height /3 - textHeight/2, textWidth, textHeight), protoMsg);
        }
        if (dataMsg != null)
        {
            GUI.Label(new Rect(0, 0, textWidth, textHeight), dataMsg);
        }
        
    }
}
