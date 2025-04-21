using System;
using System.Collections.Generic;
using UnityEngine;

public class MonoEventSystem : MonoBehaviour
{
    public static MonoEventSystem Instance { get; private set; }

    private readonly Dictionary<Type, Delegate> _mEvents = new();

    // private 

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {

    }

    private void ExecuteEvent()
    {

    }


    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        var @event = typeof(T);
        if (_mEvents.TryGetValue(@event, out var existHandlers))
        {
            _mEvents[@event] = Delegate.Combine(existHandlers, handler);
        }
        else
        {
            _mEvents[@event] = handler;
        }
    }

    public void Unsubscribe<T>(Action<T> handler) where T : IEvent
    {

    }

    public void Publish<T>(T e) where T : IEvent
    {

    }

    public void Publish<T>() where T : IEvent, new()
    {

    }
}