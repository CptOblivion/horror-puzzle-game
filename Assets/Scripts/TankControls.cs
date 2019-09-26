using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            float move = 0;
            float turn = 0;
            if (Input.GetKey(KeyCode.W))
            {
                move += 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                move -= 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                turn -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                turn += 1;
            }

            turn = turn * TurnSpeed;

            transform.Rotate(0, turn * Time.deltaTime, 0);
            anim.SetBool("Running", false);
            if (move > 0)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    move = RunSpeed;
                    anim.SetBool("Running", true);
                }
                else
                {
                    move = WalkSpeed;
                }
            }
            else if (move < 0)
            {
                move = -BackSpeed;
            }
            anim.SetFloat("Speed", move);

            if (move != 0)
            {
                Vector3 moveVec = transform.forward * move;
                characterController.Move(moveVec * Time.deltaTime);
            }
            GlobalTools.SnapToGround(characterController, groundSnapDistance);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (playerInteract.target)
                {
                    playerInteract.target.Interact();
                }
            }
        }

    }
}

