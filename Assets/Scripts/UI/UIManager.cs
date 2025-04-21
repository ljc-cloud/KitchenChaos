using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
public enum UIPanelType
{
    MessageUI,
    MainMenuUI,
    SignInUI,
    SignUpUI,
    RoomListUI,
    RoomUI,
    GameUI,
    GameOverUI,
    CreateRoomUI,
}

public enum ShowUIPanelType
{
    MoveFadeIn,
    FadeIn
}

public enum HideUIPanelType
{
    MoveFadeOut,
    FadeOut
}

public class UIManager : BaseManager
{
    public const float UISHOW_START_POSITION = 500f;
    public const float UISHOW_END_POSITION = 0f;
    public const float UIHIDE_START_POSITION = 0f;
    public const float UIHIDE_END_POSITION = -500f;
    public const float UISHOW_DURATION = .3f;

    private RectTransform _canvas;
    private UIPanelSoListSo _uiPanelSoListSo;

    public UIManager(UIPanelSoListSo uiPanelSoListSo)
    {
        _uiPanelSoListSo = uiPanelSoListSo;
    }

    private int _currentLayer;
    private readonly Dictionary<UIPanelType, BaseUIPanel> _uiPanelDict = new();
    private readonly Dictionary<UIPanelType, string> _uiPanelPathDict = new();

    private readonly Stack<BaseUIPanel> _uiPanelStack = new();
    private readonly LinkedList<BaseUIPanel> _uiPanelList = new();
    // private readonly SortedList<int, BaseUIPanel> _uiPanelList = new();

    private static readonly UIPanelType[] UnLoginUIPanelArray = { UIPanelType.SignInUI, UIPanelType.SignUpUI };

    public override void OnInit()
    {
        base.OnInit();

        _canvas = GameObject.Find("UIPanelCanvas").GetComponent<RectTransform>();
        GameObject.DontDestroyOnLoad(_canvas.gameObject);
        InitUIPanel();

        GameInterface.Interface.EventSystem.Subscribe<PlayerSignInEvent>(OnPlayerSignIn);
    }

    private void OnPlayerSignIn(PlayerSignInEvent _)
    {
        List<BaseUIPanel> uiPanelToRemove = new();
        foreach (var uiPanel in _uiPanelList)
        {
            if (UnLoginUIPanelArray.Contains(uiPanel.UIType))
            {
                uiPanelToRemove.Add(uiPanel);
            }
        }

        foreach (var uiPanel in uiPanelToRemove)
        {
            _uiPanelList.Remove(uiPanel);
        }
    }

    public override void OnDestroy()
    {
        GameInterface.Interface.EventSystem.Unsubscribe<PlayerSignInEvent>(OnPlayerSignIn);
        base.OnDestroy();
    }

    /// <summary>
    /// 显示指定UI（将栈顶UI隐藏）
    /// </summary>
    /// <param name="uiPanelType"></param>
    /// <param name="showUIPanelType"></param>
    public void PushUIPanel(UIPanelType uiPanelType, ShowUIPanelType showUIPanelType)
    {
        #region SortedList

        //
        // Debug.Log($"PushUIPanel,Type: {uiPanelType}");
        // if (_uiPanelList.Count > 0)
        // {
        //     BaseUIPanel uiPanel = _uiPanelList[_currentLayer];
        //     HideUIPanel(uiPanel.transform, HideUIPanelType.MoveFadeOut);
        //     _currentLayer = uiPanel.Layer - 1;
        // }
        //
        // // BaseUIPanel baseUIPanel;
        // if (_uiPanelDict.TryGetValue(uiPanelType, out var panel))
        // {
        //     DoShowUIPanel(panel, showUIPanelType);
        // }
        // else
        // {
        //     SpawnUIPanelAsync(uiPanelType, baseUIPanel =>
        //     {
        //         DoShowUIPanel(baseUIPanel, showUIPanelType);
        //     });
        // }

        #endregion

        #region Stack

        // 栈顶UI隐藏
        // if (_uiPanelStack.TryPeek(out BaseUIPanel peekUIPanel))
        // {
        //     HideUIPanel(peekUIPanel.transform, HideUIPanelType.FadeOut);
        // }
        //
        // PushUIPanelInternal(uiPanelType, showUIPanelType);

        #endregion


        #region LinkedList

        // 链表结尾UI隐藏
        if (_uiPanelList.Count > 0)
        {
            BaseUIPanel topUiPanel = _uiPanelList.Last.Value;
            HideUIPanelInternal(topUiPanel, HideUIPanelType.FadeOut);
        }

        PushUIPanelAppend(uiPanelType, showUIPanelType);

        #endregion
    }

    private void DoShowUIPanel(BaseUIPanel uiPanel, ShowUIPanelType showUIPanelType)
    {
        #region Stack

        // 缓存中存在，将UI对象压入栈中
        // _uiPanelStack.Push(uiPanel);
        // uiPanel.OnBeforeShow();
        // ShowUIPanel(uiPanel.transform, showUIPanelType);
        // uiPanel.OnShow();
        // uiPanel.OnAfterShow();

        #endregion

        #region LinkedList

        _uiPanelList.AddLast(uiPanel);
        ShowUIPanelInternal(uiPanel, showUIPanelType);

        #endregion
    }

    /// <summary>
    /// 显示指定UI（栈顶UI不隐藏）
    /// </summary>
    /// <param name="uiPanelType"></param>
    /// <param name="showUIPanelType"></param>
    public void PushUIPanelAppend(UIPanelType uiPanelType, ShowUIPanelType showUIPanelType)
    {
        #region SortedList

        // if (_uiPanelDict.TryGetValue(uiPanelType, out var panel))
        // {
        //     DoShowUIPanel(panel, showUIPanelType);
        // }
        // else
        // {
        //     SpawnUIPanelAsync(uiPanelType, baseUIPanel =>
        //     {
        //         DoShowUIPanel(baseUIPanel, showUIPanelType);
        //     });
        // }

        #endregion

        #region Stack

        // 栈顶UI隐藏
        // if (_uiPanelStack.TryPeek(out BaseUIPanel peekUIPanel))
        // {
        //     HideUIPanel(peekUIPanel.transform, HideUIPanelType.FadeOut);
        // }
        //
        // // 尝试在缓存中获取UI对象
        // if (_uiPanelDict.TryGetValue(uiPanelType, out BaseUIPanel uiPanel))
        // {
        //     // 缓存中存在，将UI对象压入栈中
        //     DoShowUIPanel(uiPanel, showUIPanelType);
        // }
        // else
        // {
        //     // 如果缓存中不存在改UI对象，则生成
        //     SpawnUIPanelAsync(uiPanelType, spawnedUIPanel =>
        //     {
        //         DoShowUIPanel(spawnedUIPanel, showUIPanelType);
        //     });
        // }

        #endregion

        #region LinkedList

        if (_uiPanelDict.TryGetValue(uiPanelType, out var panel))
        {
            DoShowUIPanel(panel, showUIPanelType);
        }
        else
        {
            SpawnUIPanelAsync(uiPanelType, baseUIPanel => { DoShowUIPanel(baseUIPanel, showUIPanelType); });
        }

        #endregion
    }


    /// <summary>
    /// 将栈顶UI隐藏并弹出
    /// </summary>
    public void PopUIPanel()
    {
        #region SotedList

        // // stack 栈顶ui隐藏并弹出
        // if (_uiPanelList.Count == 0) return;
        // BaseUIPanel uiPanel = _uiPanelList[_currentLayer];
        // HideUIPanel(uiPanel.transform, HideUIPanelType.MoveFadeOut);
        // _uiPanelList.Remove(uiPanel.Layer);
        // _currentLayer = uiPanel.Layer - 1;
        //
        // Debug.Log($"CurrentLayerPop:{_currentLayer}");
        //
        // if (_uiPanelList.Count == 0) return;
        // uiPanel = _uiPanelList[_currentLayer];
        // uiPanel.OnBeforeShow();
        // ShowUIPanel(uiPanel.transform, ShowUIPanelType.MoveFadeIn);
        // uiPanel.OnShow();
        // uiPanel.OnAfterShow();
        //
        // _currentLayer = uiPanel.Layer;
        // Debug.Log($"CurrentLayerPop:{_currentLayer}");

        #endregion

        #region Stack

        // // 将栈顶UI隐藏并弹出
        // if (_uiPanelStack.TryPop(out BaseUIPanel uiPanel))
        // {
        //     HideUIPanel(uiPanel.transform, HideUIPanelType.FadeOut);
        // }
        //
        // // 将现在的栈顶UI显示
        // if (_uiPanelStack.TryPeek(out uiPanel))
        // {
        //     DoShowUIPanel(uiPanel, ShowUIPanelType.FadeIn);
        // }

        #endregion

        #region LinkedList

        if (_uiPanelList.Count == 0) return;

        var lastUIPanel = _uiPanelList.Last?.Value;
        if (_uiPanelList.Count > 0 && lastUIPanel != null)
        {
            HideUIPanelInternal(lastUIPanel, HideUIPanelType.FadeOut);
            _uiPanelList.RemoveLast();
        }

        lastUIPanel = _uiPanelList.Last?.Value;
        if (_uiPanelList.Count > 0 && lastUIPanel != null)
        {
            ShowUIPanelInternal(lastUIPanel, ShowUIPanelType.FadeIn);
        }

        #endregion
    }

    /// <summary>
    /// 隐藏并移除UI
    /// </summary>
    /// <param name="uiPanelType"></param>
    public void HideAndRemoveUIPanel(UIPanelType uiPanelType)
    {
        BaseUIPanel uiPanelToRemove = null;
        foreach (var uiPanel in _uiPanelList)
        {
            if (uiPanel.UIType == uiPanelType)
            {
                HideUIPanelInternal(uiPanel, HideUIPanelType.FadeOut);
                uiPanelToRemove = uiPanel;
            }
        }

        if (uiPanelToRemove != null)
        {
            _uiPanelList.Remove(uiPanelToRemove);
        }
    }

    /// <summary>
    /// 隐藏UI
    /// </summary>
    /// <param name="uiPanelType"></param>
    public void HideUIPanel(UIPanelType uiPanelType)
    {
        foreach (var uiPanel in _uiPanelList)
        {
            if (uiPanel.UIType == uiPanelType)
            {
                HideUIPanelInternal(uiPanel, HideUIPanelType.FadeOut);
                return;
            }
        }
    }

    public void PopUIPanelNotShow()
    {

        #region SortedList

        // if (_uiPanelList.Count == 0) return;
        // BaseUIPanel uiPanel = _uiPanelList[_currentLayer];
        // HideUIPanel(uiPanel.transform, HideUIPanelType.MoveFadeOut);
        // _uiPanelList.Remove(uiPanel.Layer);
        //
        // if (_uiPanelList.Count == 0) return;
        // uiPanel = _uiPanelList[_currentLayer];
        // _currentLayer = uiPanel.Layer;

        #endregion

        #region Stack

        // 将栈顶UI隐藏并弹出
        // if (_uiPanelStack.TryPop(out BaseUIPanel uiPanel))
        // {
        //     HideUIPanel(uiPanel.transform, HideUIPanelType.FadeOut);
        // }

        #endregion

        #region LinkedList

        if (_uiPanelList.Count == 0) return;

        var lastUIPanel = _uiPanelList.Last?.Value;
        if (_uiPanelList.Count > 0 && lastUIPanel != null)
        {
            HideUIPanelInternal(lastUIPanel, HideUIPanelType.FadeOut);
            _uiPanelList.RemoveLast();
        }

        #endregion
    }

    public void ShowMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        Debug.Log("ShowMessage:" + message);
        if (_uiPanelDict.TryGetValue(UIPanelType.MessageUI, out var uiPanel))
        {
            MessageUI messageUI = uiPanel as MessageUI;
            messageUI?.ShowMessage(message);
        }
        else
        {
            SpawnUIPanelAsync(UIPanelType.MessageUI, panel =>
            {
                MessageUI messageUI = panel as MessageUI;
                messageUI?.ShowMessage(message);
            });
        }
    }

    private void ShowUIPanelInternal(BaseUIPanel uiPanel, ShowUIPanelType showUIPanelType)
    {
        uiPanel.OnShow();
        CanvasGroup canvasGroup = uiPanel.transform.GetComponent<CanvasGroup>();
        canvasGroup.DOFade(1, UISHOW_DURATION).OnComplete(() => { });
        if (showUIPanelType is ShowUIPanelType.MoveFadeIn)
        {
            uiPanel.transform.DOLocalMoveX(UISHOW_END_POSITION, UISHOW_DURATION);
        }
        else
        {
            uiPanel.transform.localPosition = new Vector3(UISHOW_END_POSITION
                , uiPanel.transform.localPosition.y, 0);
        }

        canvasGroup.blocksRaycasts = true;
    }

    private void HideUIPanelInternal(BaseUIPanel uiPanel, HideUIPanelType hideUIPanelType)
    {
        uiPanel.OnHide();

        void OnComplete()
        {
            uiPanel.transform.localPosition = new Vector3(UISHOW_START_POSITION, uiPanel.transform.localPosition.y, 0);
        }

        CanvasGroup canvasGroup = uiPanel.transform.GetComponent<CanvasGroup>();
        if (hideUIPanelType is HideUIPanelType.MoveFadeOut)
        {
            uiPanel.transform.DOLocalMoveX(UIHIDE_END_POSITION, UISHOW_DURATION);
        }

        canvasGroup.DOFade(0, UISHOW_DURATION).OnComplete(OnComplete);
        canvasGroup.blocksRaycasts = false;
    }

    private BaseUIPanel SpawnUIPanel(UIPanelType uiPanelType)
    {
        if (_uiPanelPathDict.TryGetValue(uiPanelType, out string path))
        {
            Debug.Log($"Spawn UI, Type:{uiPanelType}, Path:{path}");
            GameObject uiPrefab = Resources.Load<GameObject>(path);
            GameObject uiGameObject = GameObject.Instantiate(uiPrefab, _canvas);
            BaseUIPanel baseUIPanel = uiGameObject.GetComponent<BaseUIPanel>();
            baseUIPanel.UIType = uiPanelType;
            _uiPanelDict[uiPanelType] = baseUIPanel;
            CanvasGroup canvasGroup = uiGameObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            baseUIPanel.OnInit();
            return baseUIPanel;
        }
        else
        {
            Debug.LogError($"UI Panel Type {uiPanelType} path not found");
            return null;
        }
    }

    private void SpawnUIPanelAsync(UIPanelType uiPanelType, Action<BaseUIPanel> onComplete)
    {
        if (_uiPanelPathDict.TryGetValue(uiPanelType, out string path))
        {
            Debug.Log($"Spawn UI, Type:{uiPanelType}, Path:{path}");
            ResourceRequest resourceRequest = Resources.LoadAsync<GameObject>(path);
            resourceRequest.completed += _ =>
            {
                GameObject uiPrefab = resourceRequest.asset as GameObject;
                GameObject uiGameObject = GameObject.Instantiate(uiPrefab, _canvas);
                BaseUIPanel baseUIPanel = uiGameObject.GetComponent<BaseUIPanel>();
                baseUIPanel.UIType = uiPanelType;
                _uiPanelDict[uiPanelType] = baseUIPanel;
                CanvasGroup canvasGroup = uiGameObject.GetComponent<CanvasGroup>();
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
                baseUIPanel.OnInit();
                onComplete?.Invoke(baseUIPanel);

            };
        }
        else
        {
            Debug.LogError($"UI Panel Type {uiPanelType} path not found");
        }
    }

    private void InitUIPanel()
    {
        foreach (var uiPanelSo in _uiPanelSoListSo.uIPanelSoList)
        {
            Debug.Log($"UIAdd,Type:{uiPanelSo.uIPanelType},Path:{uiPanelSo.path}");
            _uiPanelPathDict.Add(uiPanelSo.uIPanelType, uiPanelSo.path);
        }
    }
}