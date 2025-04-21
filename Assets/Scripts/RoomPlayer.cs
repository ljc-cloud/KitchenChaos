using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshPro nickNameText;
    [SerializeField] private Image readyImage;
    [SerializeField] private MeshRenderer headMeshRenderer;
    [SerializeField] private MeshRenderer bodyMeshRenderer;


    private const string READY_TEXT = "准备";
    private const string UNREADY_TEXT = "取消准备";

    public int RoomIndex { get; private set; }

    public void SetRoomPlayer(int index, string nickname, Color color)
    {
        RoomIndex = index;
        nickNameText.text = nickname;
        headMeshRenderer.material.color = color;
        bodyMeshRenderer.material.color = color;
    }

    public void SetReady(bool ready)
    {
        readyImage.enabled = ready;
    }
}