using UnityEngine;
using CustomInspector;
using System;


public class Stomp : MonoBehaviour
{
    [SelfFill][SerializeField] Rigidbody rb;
    [SerializeField] KeyCode stompInputKey = KeyCode.Q;
    [SerializeField] float stompForce = 10f;

    [ReadOnly][SerializeField]JumpState jumpState;

    public static Action<float> OnStompDown;

   
    private void OnEnable()
    {
        Jump.OnJumpStateChanged += GetCurrentJumpState;
    }

    private void OnDisable()
    {
        Jump.OnJumpStateChanged -= GetCurrentJumpState;
    }

    private void Update()
    {
       StompDown();
    }

    void StompDown()
    {
        if (jumpState == JumpState.Grounded) return;

        if (Input.GetKeyDown(stompInputKey))
        {
            OnStompDown?.Invoke(stompForce);
        }   
    }
    void GetCurrentJumpState(JumpState state) => jumpState = state;
}
