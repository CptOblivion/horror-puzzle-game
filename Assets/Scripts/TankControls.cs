using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankControls : MonoBehaviour
{
    public float WalkSpeed = 3;
    public float BackSpeed = 1;
    public float RunSpeed = 5;
    public float TurnSpeed = 1;
    public float groundSnapDistance = .5f;
    public Animator anim;

    CharacterController characterController;
    PlayerInteract playerInteract;
    Vector3 Velocity;
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInteract = GetComponentInChildren<PlayerInteract>();
    }

    private void Start()
    {
        GlobalTools.player = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GlobalTools.WasPaused)
        {
            Vector2 MoveInputs = GlobalTools.inputsGameplay.FindAction("Move").ReadValue<Vector2>();

            MoveInputs.x = MoveInputs.x * TurnSpeed;

            transform.Rotate(0, MoveInputs.x * Time.deltaTime, 0);
            anim.SetBool("Running", false);
            if (MoveInputs.y > 0)
            {
                if (GlobalTools.inputsGameplay.FindAction("Run").ReadValue<float>() > 0)
                {
                    MoveInputs.y = RunSpeed;
                    anim.SetBool("Running", true);
                }
                else
                {
                    MoveInputs.y = WalkSpeed;
                }
            }
            else if (MoveInputs.y < 0)
            {
                MoveInputs.y = -BackSpeed;
            }
            anim.SetFloat("Speed", MoveInputs.y);

            if (MoveInputs.y != 0)
            {
                Vector3 moveVec = transform.forward * MoveInputs.y;
                characterController.Move(moveVec * Time.deltaTime);
            }
            GlobalTools.SnapToGround(characterController, groundSnapDistance);

            if (GlobalTools.inputsGameplay.FindAction("Interact").triggered)
            {
                if (playerInteract.target)
                {
                    playerInteract.target.Interact();
                }
            }
        }

    }
}

