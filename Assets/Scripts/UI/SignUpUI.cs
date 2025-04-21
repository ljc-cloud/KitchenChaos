using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpUI : BaseUIPanel
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField nicknameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button signUpButton;
    [SerializeField] private Button backButton;

    private SignUpRequest _mSignUpRequest;

    private void Start()
    {
        signUpButton.onClick.AddListener(SignUpClick);
        backButton.onClick.AddListener(BackClick);
    }

    public override void OnInit()
    {
        base.OnInit();
        transform.localPosition = new Vector3(UIManager.UISHOW_START_POSITION
            , transform.localPosition.y, 0);

        _mSignUpRequest = GameInterface.Interface.RequestManager.GetRequest<SignUpRequest>();
    }

    private void BackClick()
    {
        GameInterface.Interface.UIManager.PopUIPanel();
    }

    private void SignUpClick()
    {
        string username = usernameInput.text;
        string nickname = nicknameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username is empty");
            return;
        }

        if (string.IsNullOrEmpty(nickname))
        {
            Debug.LogError("Nickname is empty");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            Debug.LogError("Password is empty");
            return;
        }

        _mSignUpRequest.SendSignUpRequest(username, nickname, password,
            () =>
            {
                Invoker.Instance.DelegateList.Add(() =>
                {
                    GameInterface.Interface.UIManager.PushUIPanel(UIPanelType.SignInUI, ShowUIPanelType.MoveFadeIn);
                });
            });
    }
}