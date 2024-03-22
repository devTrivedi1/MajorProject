using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameTimer : MonoBehaviour
{
    [SerializeField] float colorChangeIntervel;
    [SerializeField] float colorChangeDuration;
    [SerializeField] Color ColorChangeIntervelColor;

    [SerializeField] TimerUtilities gameTimer;


    private void Start()
    {
        gameTimer = new TimerUtilities(gameTimer.showDecimal, GetComponent<TextMeshProUGUI>());
        gameTimer.InitializeUnlimitedStopWatch();

        GameManager.Instance?.onWin.AddListener(() => gameTimer.EndStopWatch(true));
        GameManager.Instance?.onLose.AddListener(() => gameTimer.EndStopWatch(true));
    }

    private void OnDisable()
    {
        GameManager.Instance?.onWin.RemoveListener(() => gameTimer.EndStopWatch(true));
        GameManager.Instance?.onLose.RemoveListener(() => gameTimer.EndStopWatch(true));
    }

    private void Update()
    {
        gameTimer.StartUnlimitedStopWatch();
        gameTimer.ChangeTextColor(colorChangeIntervel, ColorChangeIntervelColor, colorChangeDuration);
    }
}
