using UnityEngine;


public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer headMeshRenderer;
    [SerializeField] private MeshRenderer bodyMeshRenderer;

    public void SetPlayerVisual(Color color)
    {
        headMeshRenderer.material.color = color;
        bodyMeshRenderer.material.color = color;
    }
}