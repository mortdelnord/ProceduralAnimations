using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProtoMove : MonoBehaviour
{
    public InputActionAsset playerInputs;
    private InputAction moveInput;

    public float speed;
    private Vector2 axis;


    private void Start()
    {
        playerInputs.FindActionMap("Player").Enable();
        moveInput = playerInputs.FindActionMap("Player").FindAction("Move");
    }

    private void Update()
    {
        axis = moveInput.ReadValue<Vector2>();
        Debug.Log(axis);
        axis.y *= speed;
        axis.x *= speed;
        Debug.Log(axis + "Axis with speed");

        transform.Translate(transform.forward * axis.y + transform.right * axis.x);
    }

   
}
