using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene 
{
    MainMenuScene,
    RoomScene,
    LoadingScene,
    GameScene,
    GameOverScene,
}

public class SceneLoader
{
    public event Action OnSceneLoad;
    public event Action OnSceneLoadComplete;
    
    public void LoadScene(Scene scene)
    {
        OnSceneLoad?.Invoke();
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
        
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);
        loadSceneAsync.completed += _ => OnSceneLoadComplete?.Invoke(); 
    }

    public AsyncOperation LoadGameSceneAsync()
    {
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(Scene.GameScene.ToString(), LoadSceneMode.Single);
        // loadSceneAsync.allowSceneActivation = false;
        return loadSceneAsync;
    }

    public void LoadSceneAsync(Scene scene, Action onComplete = null)
    {
        OnSceneLoad?.Invoke();

        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);

        loadSceneAsync.completed += _ =>
        {
            OnSceneLoadComplete?.Invoke();
            onComplete?.Invoke();
        };
    }
}