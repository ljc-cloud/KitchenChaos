using TMPro;
using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI tipText;

    private int _mNowProgress;
    private int _mTargetProgress;
    private AsyncOperation _mLoadSceneAsyncOperation;

    private bool _mRequestSend;
    
    private void Start()
    {
        tipText.gameObject.SetActive(false);
        _mLoadSceneAsyncOperation = GameInterface.Interface.SceneLoader.LoadGameSceneAsync();
        _mLoadSceneAsyncOperation.allowSceneActivation = false;
        _mRequestSend = false;
    }

    private void Update()
    {
        if (_mLoadSceneAsyncOperation == null)
        {
            return;
        }

        if (_mLoadSceneAsyncOperation.progress < 0.9f)
        {
            _mTargetProgress = (int)(_mLoadSceneAsyncOperation.progress * 100);
        }
        else
        {
            _mTargetProgress = 100;
        }

        if (_mNowProgress < _mTargetProgress)
        {
            _mNowProgress++;
        }
        progressText.text = $"{_mNowProgress}%";
        if (_mNowProgress == 100 && !_mRequestSend)
        {
            Debug.Log("LoadComplete!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            LoadGameSceneCompleteRequest request = GameInterface.Interface.RequestManager.GetRequest<LoadGameSceneCompleteRequest>();
            request.SendLoadGameSceneCompleteRequest(onSuccess: OnAllPlayerLoadComplete);
            _mRequestSend = true;
            tipText.gameObject.SetActive(true);
        }
    }

    private void OnAllPlayerLoadComplete()
    {
        _mLoadSceneAsyncOperation.allowSceneActivation = true;
    }
}