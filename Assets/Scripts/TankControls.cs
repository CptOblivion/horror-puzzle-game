using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankControls : MonoBehaviour
{
    CharacterController characterController;
    public float WalkSpeed = 3;
    public float BackSpeed = 1;
    public float RunSpeed = 5;
    public float TurnSpeed = 1;
    public float groundSnapDistance = .5f;
    public Animator anim;
    Vector3 Velocity;
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
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

        if (!characterController.isGrounded)
        {
            float feetDist = (characterController.height/2) - characterController.radius;
            Vector3 feetPos = transform.position + new Vector3(0, -feetDist, 0);
            RaycastHit hit;
            if (Physics.SphereCast(feetPos, characterController.radius, Vector3.down, out hit, groundSnapDistance))
            {

                float dropDistance = hit.distance;
                transform.position = transform.position + new Vector3 (0, -(dropDistance - characterController.skinWidth), 0);
            }
        }

    }
}
