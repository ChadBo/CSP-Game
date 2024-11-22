using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Vector2 Movement;
    public static float Attack;
    public static float Roll;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction attackInput;
    private InputAction rollInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        attackInput = playerInput.actions["Attack"];
        rollInput = playerInput.actions["Roll"];
    }

    private void Update()
    {
        Movement = moveAction.ReadValue<Vector2>();
        Attack = attackInput.ReadValue<float>();
        Roll = rollInput.ReadValue<float>();
    }
}
