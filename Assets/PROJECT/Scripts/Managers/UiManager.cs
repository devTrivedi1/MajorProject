using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject pauseScreenCanvas;

    public void ShowWinScreen()
    {
        winScreen.SetActive(true);
    }

    public void ShowLoseScreen()
    {
        loseScreen.SetActive(true);
    }

    public void ShowPauseScreen()
    {
        pauseScreenCanvas.SetActive(true);
    }

    public void HidePauseScreen()
    {
        pauseScreenCanvas.SetActive(false);
    }
}
