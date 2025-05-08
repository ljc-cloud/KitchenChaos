using System;
using System.Collections;
using System.Collections.Generic;
using GameFrameSync;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour {

    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;


    public enum State {
        NotStarted,
        WaitingStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }


    private State state;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = int.MaxValue;
    private bool isGamePaused = false;  
    
    private void Awake() {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        state = State.NotStarted;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInterface.Interface.EventSystem.Subscribe<ChangeGameStateEvent>(OnChangeGameState);
    }

    private void OnChangeGameState(ChangeGameStateEvent e)
    {
        ChangeCurrentGameState(e.newState);
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        TogglePauseGame();
    }

    private void Update() {
        switch (state) {
            case State.NotStarted:
                break;
            case State.CountdownToStart:
                // countdownToStartTimer -= Time.deltaTime;
                // if (countdownToStartTimer < 0f) {
                //     state = State.GamePlaying;
                //     gamePlayingTimer = gamePlayingTimerMax;
                //     OnStateChanged?.Invoke(this, EventArgs.Empty);
                // }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                // Debug.Log($"Game playing timer: {gamePlayingTimer}");
                if (gamePlayingTimer < 0f)
                {
                    Debug.Log("GameOver");
                    ChangeCurrentGameState(State.GameOver);
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
    }

    public bool IsGamePlaying() {
        return state == State.GamePlaying;
    }

    public bool IsCountdownToStartActive() {
        return state == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer() {
        return countdownToStartTimer;
    }

    public bool IsGameOver() {
        return state == State.GameOver;
    }

    public float GetGamePlayingTimerNormalized() {
        return gamePlayingTimer / gamePlayingTimerMax;
    }

    public float GetGamePlayingLeftTimer()
    {
        return gamePlayingTimer;
    }

    private void ChangeCurrentGameState(State newState)
    {
        state = newState;
        GameInterface.Interface.EventSystem.Publish(new GameStateChangedEvent
        {
            state = newState
        });
        switch (state)
        {
            case State.NotStarted:
                break;
            case State.CountdownToStart:
                gamePlayingTimer = gamePlayingTimerMax;
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                Debug.Log($"Game playing timer: {gamePlayingTimer}");
                if (gamePlayingTimer < 0f)
                {
                    Debug.Log("GameOver");
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
    }
    

    public void TogglePauseGame() {
        isGamePaused = !isGamePaused;
        if (isGamePaused) {
            Time.timeScale = 0f;

            OnGamePaused?.Invoke(this, EventArgs.Empty);
        } else {
            Time.timeScale = 1f;

            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

}