using TMPro;
using UnityEngine;


public class PlayerCoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI playerNicknameText;

    private int _mPlayerId;

    public void UpdateCoin(int coin)
    {
        coinText.text = coin.ToString();
    }

    public void InitCoinUI(int playerId, string playerNickname)
    {
        _mPlayerId = playerId;
        playerNicknameText.text = playerNickname;
    }
}