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
    }

    private void Update()
    {
        gameTimer.StartUnlimitedStopWatch();
        gameTimer.ChangeTextColor(colorChangeIntervel, ColorChangeIntervelColor, colorChangeDuration);
    }
}
