using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerDisplayUI : MonoBehaviour
    {
        [SerializeField] private Image crownImage;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI playerNicknameText;
        [SerializeField] private Sprite no1Sprite;
        [SerializeField] private Sprite no2Sprite;
        public void SetPlayerDisplayUI(int rank, string nickname, int coin)
        {
            if (rank == 0)
            {
                crownImage.sprite = no1Sprite;
            }
            else if (rank == 1)
            {
                crownImage.sprite = no2Sprite;
            }
            else
            {
                crownImage.enabled = false;
            }
            
            playerNicknameText.text = nickname;
            coinText.text = coin.ToString();
        }
    }
}