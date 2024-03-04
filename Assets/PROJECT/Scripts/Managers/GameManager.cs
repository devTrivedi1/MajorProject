using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Events")]
    public UnityEvent onWin;
    public UnityEvent onLose;
    public UnityEvent onPause;
    public UnityEvent onResume;
    bool isGamePaused = false;
    bool hasGameEnded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (hasGameEnded) Time.timeScale = 0;
    }

    public void Win()
    {
        onWin.Invoke();
        EndGame();
    }

    public void Lose()
    {
        if (!hasGameEnded)
        {
            onLose.Invoke();
            EndGame();
        }
    }

    public void TogglePause()
    {
        if (!hasGameEnded)
        {
            isGamePaused = !isGamePaused;

            if (isGamePaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    private void PauseGame()
    {
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0;
        }
        onPause.Invoke();
    }

    private void ResumeGame()
    {
        onResume.Invoke();
    }

    private void EndGame()
    {
        hasGameEnded = true;
        Time.timeScale = 0;
    }
}
