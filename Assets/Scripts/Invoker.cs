using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity Api调用器
/// </summary>
public class Invoker : MonoBehaviour
{
    public static Invoker Instance { get; private set; }

    public List<Action> DelegateList { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DelegateList = new List<Action>();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        Execute();
    }

    private void Execute()
    {
        if (DelegateList.Count == 0) return;
        for (int i = 0; i < DelegateList.Count; i++)
        {
            try
            {
                Debug.Log("Invoker Execute!!!!!");
                DelegateList[i]?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        DelegateList.Clear();
    }
}