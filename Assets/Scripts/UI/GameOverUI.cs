using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;

    private void Start() {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;

        playAgainButton.onClick.AddListener(() =>
        {
            // TODO: Play again
            
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            // TODO: Back to mainMenu
            GameInterface.Interface.SceneLoader.LoadScene(Scene.MainMenuScene);
        });
        
        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (KitchenGameManager.Instance.IsGameOver()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }


}