﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticReplayScript
{
    public static string URL = "https://calculated.gg/api/replay/EACC3A5811E8E0AE177575BC6D034FCF/positions";

    public static RootObject gameData;
    public static Proto proto;
}

[System.Serializable]
public class RootObject
{
    public List<List<double>> ball;
    public List<List<List<object>>> players;
    public List<int> colors;
    public List<string> names;
    public List<List<double>> frames;
    public string id;
}