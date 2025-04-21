using System;
using UnityEngine;


public class GameAssets : MonoBehaviour
{
    public static GameAssets Instance { get; private set; }

    public const int BIRD_LAYER = 6;
    public const int WALL_LAYER = 7;
    public const int AREA_LAYER = 8;

    public const int WAIT_FRAMES = 5;

    public const string AUTHORIZATION_KEY = "authorization";

    public const int COUNT_DOWN = 3;

    public const double RECONNECT_INTERVAL_MS = 5000d;
    public const int RECONNECT_COUNT_MAX = 5;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
}