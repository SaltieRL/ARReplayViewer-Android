using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ReplayScript : MonoBehaviour {

	// Use this for initialization
    //private static string BASE_URL = "https://calculated.gg/api/replay";
    private static string BASE_URL = "https://calculated.gg/api/replay/EACC3A5811E8E0AE177575BC6D034FCF/positions";
    public GameObject prefab;
    void Start ()
	{

    }



    // Update is called once per frame
    void Update () {
		
	}
}
[System.Serializable]
class ReplayData
{
    public List<float>[] ball;
    public List<List<List<float>>> players;
    public List<int> colors;
    public List<string> names;
    public List<List<float>> frames;
    public string id;
}
