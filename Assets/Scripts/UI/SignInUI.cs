using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SignInUI : BaseUIPanel
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button signInButton;
    [SerializeField] private Button gotoSignUpButton;
    [SerializeField] private Button closeButton;

    private SignInRequest _mSignInRequest;

    public override void OnInit()
    {
        _mSignInRequest = GameInterface.Interface.RequestManager.GetRequest<SignInRequest>();
        base.OnInit();
    }

    private void Start()
    {
        signInButton.onClick.AddListener(SignInClick);
        closeButton.onClick.AddListener(Close);
        gotoSignUpButton.onClick.AddListener(GotoSignUp);
    }

    public override void OnBeforeShow()
    {
        base.OnInit();
        transform.localPosition = new Vector3(UIManager.UISHOW_START_POSITION
            , transform.localPosition.y, 0);
    }

    private void GotoSignUp()
    {
        GameInterface.Interface.UIManager.PushUIPanel(UIPanelType.SignUpUI, ShowUIPanelType.MoveFadeIn);
    }

    private void Close()
    {
        GameInterface.Interface.UIManager.PopUIPanel();
    }

    private void SignInClick()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        Debug.Log("字符编码：" + username);

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username is empty");
            GameInterface.Interface.UIManager.ShowMessage("用户名为空");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            Debug.LogError("Password is empty");
            GameInterface.Interface.UIManager.ShowMessage("密码为空");
            return;
        }

        _mSignInRequest.SendSignInRequest(username, password, OnSignInSuccess, OnSignInFail);
    }

    private void OnSignInSuccess()
    {
        // GameInterface.Interface.SceneLoader.LoadScene(Scene.GameScene);
        string username = usernameInput.text;
        string password = passwordInput.text;

        Authorization authorization = new Authorization()
        {
            username = username,
            password = password
        };

        string json = JsonUtility.ToJson(authorization);
        PlayerPrefs.SetString(GameAssets.AUTHORIZATION_KEY, json);
        PlayerPrefs.Save();
        Debug.Log("已存储登录凭证！");

        GameInterface.Interface.UIManager.PopUIPanel();
        GameInterface.Interface.UIManager.PushUIPanel(UIPanelType.RoomListUI, ShowUIPanelType.FadeIn);
    }

    private void OnSignInFail()
    {

    }
}