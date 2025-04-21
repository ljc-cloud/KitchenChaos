using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour {


    [SerializeField] private Image timerImage;
    [SerializeField] private Image progressImage;
    [SerializeField] private TextMeshProUGUI timeText;

    private void Start()
    {
        progressImage.fillAmount = KitchenGameManager.Instance.GetGamePlayingTimerNormalized();
        var leftTime = KitchenGameManager.Instance.GetGamePlayingLeftTimer();
        int minutes = (int)leftTime / 60;
        int seconds = (int)leftTime % 60;
        if (seconds < 10)
        {
            timeText.text = $"{minutes}:0{seconds}";
        }
        else
        {
            timeText.text = $"{minutes}:{seconds}";
        }
    }

    private void Update() {
        progressImage.fillAmount = KitchenGameManager.Instance.GetGamePlayingTimerNormalized();
        var leftTime = KitchenGameManager.Instance.GetGamePlayingLeftTimer();
        int minutes = (int)leftTime / 60;
        int seconds = (int)leftTime % 60;
        if (seconds < 10)
        {
            timeText.text = $"{minutes}:0{seconds}";
        }
        else
        {
            timeText.text = $"{minutes}:{seconds}";
        }
        
    }
}