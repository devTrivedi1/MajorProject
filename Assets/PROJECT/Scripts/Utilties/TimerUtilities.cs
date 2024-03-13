using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

[System.Serializable]
public class TimerUtilities

{
    [SerializeField] public bool showDecimal;
    [SerializeField] public TextMeshProUGUI textReference;
    [SerializeField] public string addonText;
    Color textStartColor;

    // holds the time reference for the type of time , it could be elapsedTime or timeLeft
    float timeReference;
    float elapsedTime;
    float targetStopTime;
    float timeLeft;

    bool stopWatchEnabled = false;


    public TimerUtilities()
    {

    }

    public TimerUtilities(bool _showDecimal, TextMeshProUGUI _textReference)
    {
        showDecimal = _showDecimal;
        textReference = _textReference;
    }

    public void InitializeUnlimitedStopWatch()
    {
        stopWatchEnabled = true;
        elapsedTime = 0;
        textStartColor = textReference is not null ? textReference.color : Color.white;
    }

    public void InitializeNormalStopWatch(float _targetStopTime)
    {
        stopWatchEnabled = true;
        elapsedTime = 0;
        targetStopTime = _targetStopTime;
        textStartColor = textReference is not null ? textReference.color : Color.white;
    }

    public void InitializeCountDownTimer(float _timeLeft)
    {
        timeLeft = _timeLeft;
        textStartColor = textReference is not null ? textReference.color : Color.white;
    }

    public float StartUnlimitedStopWatch()
    {
        if (stopWatchEnabled == false) { return elapsedTime; }
        elapsedTime += Time.deltaTime;
        timeReference = elapsedTime;
        SetTimerText(textReference, elapsedTime);
        return elapsedTime;
    }

    public float StartStopWatch(bool _resetWhenDone)
    {
        if (elapsedTime <= targetStopTime && stopWatchEnabled)
        {
            elapsedTime += Time.deltaTime;
            timeReference = elapsedTime;
            SetTimerText(textReference, elapsedTime);
            return elapsedTime;
        }
        else
        { EndStopWatch(_resetWhenDone); }

        return elapsedTime;
    }

    public float EndStopWatch(bool resetElapsedTime)
    {
        stopWatchEnabled = false;
        elapsedTime = resetElapsedTime ? 0 : elapsedTime;
        return elapsedTime;
    }

    public float StartCountDown()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timeReference = timeLeft;
            SetTimerText(textReference, timeLeft);
        }
        return timeLeft;
    }

    public async void ChangeTextColor(float colorChangeInterval, Color newColor, float colorChangeDuration)
    {
        if (textReference is null) return;
        if (textReference.color == newColor) return;
        if ((int)timeReference % colorChangeInterval is not 0 || (int)timeReference is 0) return;

        textReference.color = newColor;

        await Task.Delay((int)(colorChangeDuration * 1000));
        textReference.color = textStartColor;
    }

    void SetTimerText(TextMeshProUGUI _textReference, float _timeReference)
    {
        if (_textReference is null) return;
        _textReference.text = showDecimal ? addonText + _timeReference.ToString("F2") : addonText + _timeReference.ToString("F0");
    }
}
