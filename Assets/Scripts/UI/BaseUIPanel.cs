using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BaseUIPanel : MonoBehaviour
{
    public UIPanelType UIType { get; set; }
    public bool Active { get; set; } = true;
    public int Layer { get; set; }

    public virtual void OnInit()
    {
        Active = true;
    }

    public virtual void OnBeforeShow()
    {

    }

    public virtual void OnShow()
    {

    }

    public virtual void OnAfterShow()
    {

    }

    public virtual void OnHide()
    {

    }
}