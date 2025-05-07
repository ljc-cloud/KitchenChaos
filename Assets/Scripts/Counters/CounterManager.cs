using System;
using System.Collections.Generic;
using UnityEngine;

namespace Counters
{
    public class CounterManager : MonoBehaviour
    {
        public static CounterManager Instance { get; private set; }
        private readonly List<BaseCounter> _mCounters = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void AddCounter(BaseCounter counter)
        {
            _mCounters.Add(counter);
        }


        public BaseCounter GetCounterFromId(int id)
        {
            BaseCounter result = null;
            foreach (var counter in _mCounters)
            {
                if (counter.CounterId == id)
                {
                    result = counter;
                    break;
                }
            }
            return result;
        }
        
    }
}