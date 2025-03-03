using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class InputManager : MonoBehaviour
{
    public static Vector2 Movement;
    public static float Attack;
    public static float Roll;
    public static float Sprint;
    public static float Interact;
    public static float Heal;
    public static float Menu;
    //
    private PlayerInput playerInput;

    private InputAction moveAction;

    private InputAction attackInput;
    private bool attackHeld;
    public static bool IsTapAttack = true;
    private const float minimumHeldDuration = 0.4f;
    private float attackPressedTime = 0;


    private InputAction rollInput;
    private InputAction sprintInput;

    private InputAction interactInput;
    private InputAction healInput;

    private InputAction MenuInput;

    private void Update()
    {
        Movement = moveAction.ReadValue<Vector2>();
        Attack = attackInput.ReadValue<float>();
        Roll = rollInput.ReadValue<float>();
        Sprint = sprintInput.ReadValue<float>();
        Interact = interactInput.ReadValue<float>();
        Heal = healInput.ReadValue<float>();
        Menu = MenuInput.ReadValue<float>();
        //
        CheckIfTappedOrHeld();
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        attackInput = playerInput.actions["Attack"];
        rollInput = playerInput.actions["Roll"];
        sprintInput = playerInput.actions["Sprint"];
        interactInput = playerInput.actions["Interact"];
        healInput = playerInput.actions["Heal"];
        MenuInput = playerInput.actions["Open Stickers"];
    }

    private void CheckIfTappedOrHeld()
    {
        if(attackInput.WasPressedThisFrame())
        {
            attackPressedTime = Time.timeSinceLevelLoad;
            attackHeld = false;
        }
        else if (attackInput.WasReleasedThisFrame())
        {
            if(!attackHeld)
            {
                IsTapAttack = true;
            }
            attackHeld = false;
        }

        if (Attack == 1)
        {
            if(Time.timeSinceLevelLoad - attackPressedTime > minimumHeldDuration)
            {
                attackHeld = true;
                IsTapAttack = false;
            }
        }
    }
}