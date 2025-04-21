using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : BaseUIPanel {


    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;


    private void Awake() {
        playButton.onClick.AddListener(() => {
            // Loader.Load(Loader.Scene.GameScene);
            // GameInterface.Interface.UIManager.PushUIPanel(UIPanelType.SignInUI, ShowUIPanelType.FadeIn);
            GameInterface.Interface.UIManager.PushUIPanelAppend(
                GameInterface.Interface.TcpClient.IsOnline ? UIPanelType.RoomListUI : UIPanelType.SignInUI,
                ShowUIPanelType.FadeIn);
        });
        quitButton.onClick.AddListener(() => {
            GameInterface.Interface.TcpClient.CloseSocket();
            Application.Quit();
        });

        Time.timeScale = 1f;
    }

}