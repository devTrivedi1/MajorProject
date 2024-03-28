using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateSettings : MonoBehaviour
{
    [SerializeField] [FixedValues(20,30,60,90,120,144,240,400)] int frameRate = 60;

    private void Awake()
    {
        Application.targetFrameRate = frameRate;
    }
}
