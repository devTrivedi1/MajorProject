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
    Resetter resetter;
    bool isGamePaused = false;
    bool hasGameEnded = false;
    float timeScale;

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
        resetter = FindObjectOfType<Resetter>();
        PlayerHealth.OnPlayerDeath += Lose;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= Lose;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Restart();
        }
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
            timeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        onPause.Invoke();
    }

    private void ResumeGame()
    {
        Time.timeScale = timeScale;
        onResume.Invoke();
    }

    public void Restart()
    {
        Time.timeScale = 1;

        if (resetter != null)
        {
            resetter.ResetAll();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void EndGame()
    {
        hasGameEnded = true;
        Time.timeScale = 0;
    }

    public void QuitGame() => Application.Quit();
}
