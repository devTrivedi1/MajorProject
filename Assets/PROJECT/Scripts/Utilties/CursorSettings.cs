using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityRandom = UnityEngine.Random;

public class CursorSettings : MonoBehaviour
{
    [SerializeField] CursorLockMode cursorLockMode = CursorLockMode.None;
    [SerializeField] bool cursorVisible = true;

    private void Start()
    {
        Cursor.lockState = cursorLockMode;
        Cursor.visible = cursorVisible;
    }
}