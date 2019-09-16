using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GlitchMonster : MonoBehaviour
{
    public float DetectRadius = 10;
    public float WalkSpeed = 1;
    public float WalkPause = .25f;
    public float WalkPauseRand = .25f;
    public float TurnSpeed = 100;
    public float LeashLength = 10; //how far it'll go from its spawn
    public float WakeTime = 1;//how long it takes to activate after detecting player
    public float WakeTurnSpeed = .3f;
    public float HomeThreshold = .01f;
    public float GroundSnapDistance = 1;
    SphereCollider detectPlayer;

    enum AIState { Idle, Activating, FollowingPlayer, FacingHome, ReturningHome, FacingIdle}
    AIState aiState;

    CharacterController characterController;
    Animator animator;
    float ActivateTimer;
    float WalkPauseTimer;
    Vector3 SpawnPosition;
    Quaternion SpawnRotation;
    NavMeshPath path;
    Vector3 LocalGoal;
    Vector3 GlobalGoal;

    private void Awake()
    {
        detectPlayer = GetComponentInChildren<SphereCollider>();
        characterController = GetComponent <CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }
    void Start()
    {
        aiState = AIState.Idle;
        SpawnPosition = transform.position;
        SpawnRotation = transform.rotation;
        detectPlayer.radius = DetectRadius;
        ActivateTimer = 0;
        animator.SetFloat("WalkSpeed", WalkSpeed);
        path = new NavMeshPath();
    }
    void OnTriggerStay(Collider other)
    {
        if ((aiState != AIState.FollowingPlayer && aiState != AIState.Activating) && other.gameObject == GlobalTools.player && PlayerLeash() < LeashLength)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (other.transform.position - transform.position), out hit))
            {
                if (hit.collider.gameObject == GlobalTools.player)
                {
                    ChangeAIState(AIState.Activating);
                }
            }

        }

    }
    // Update is called once per frame
    void Update()
    {
        if (aiState == AIState.Activating)
        {
            float LeashCurrent = (transform.position - SpawnPosition).magnitude;
            if (LeashCurrent > LeashLength || PlayerLeash() > LeashLength)
            {
                if (LeashCurrent <= HomeThreshold) ChangeAIState(AIState.FacingIdle);
                else ChangeAIState(aiState = AIState.FacingHome);
            }
            ActivateTimer -= Time.deltaTime;
            TurnToFace(GlobalTools.player.transform.position, WakeTurnSpeed);
            if (ActivateTimer <= 0) ChangeAIState(AIState.FollowingPlayer);
        }
        else if (aiState == AIState.FollowingPlayer)
        {
            float LeashCurrent = (transform.position - SpawnPosition).magnitude;
            if (LeashCurrent > LeashLength || PlayerLeash() > LeashLength)
            {
                if (LeashCurrent <= HomeThreshold) ChangeAIState(AIState.FacingIdle);
                else ChangeAIState(aiState = AIState.FacingHome);
            }
            else
            {
                UpdateWalk(GlobalTools.player.transform.position);
            }
        }
        else if (aiState == AIState.FacingHome)
        {
            if (ActivateTimer > 0)
            {
                ActivateTimer -= Time.deltaTime;
                TurnToFace(SpawnPosition, WakeTurnSpeed);
            }
            else
            {
                ChangeAIState(AIState.ReturningHome);
            }
        }
        else if (aiState == AIState.ReturningHome)
        {
            UpdateWalk(SpawnPosition);
            float LeashCurrent = (transform.position - SpawnPosition).magnitude;
            if (LeashCurrent <= HomeThreshold)
            {
                ChangeAIState(AIState.FacingIdle);
            }
        }
        else if (aiState == AIState.FacingIdle)
        {
            if (ActivateTimer <= 0)
            {
                ChangeAIState(AIState.Idle);
                transform.rotation = SpawnRotation;
            }
            else
            {
                ActivateTimer -= Time.deltaTime;
                transform.rotation = Quaternion.Lerp(transform.rotation, SpawnRotation, WakeTurnSpeed * Time.deltaTime);
            }
        }
    }

    void ChangeAIState(AIState newState)
    {
        aiState = newState;
        //Debug.Log(newState.ToString());
        if (newState == AIState.Idle)
        {
        }
        else if (newState == AIState.Activating)
        {
            ActivateTimer = WakeTime;
            animator.SetBool("Walking", false);
        }
        else if (newState == AIState.FollowingPlayer)
        {
            animator.SetBool("Walking", true);
            WalkPauseTimer = .001f; //set tiny value so first walking frame will calculate a new path

        }
        else if (newState == AIState.FacingHome)
        {
            ActivateTimer = WakeTime;
            animator.SetBool("Walking", false);
        }
        else if (newState == AIState.ReturningHome)
        {
            animator.SetBool("Walking", true);
            WalkPauseTimer = .001f;
        }
        else if (newState == AIState.FacingIdle)
        {
            ActivateTimer = WakeTime;
            animator.SetBool("Walking", false);
        }
    }
    void TurnToFace(Vector3 TargetPosition, float Speed = 1, bool XZ = true)
    {
        if (XZ)
        {
            TargetPosition.y = transform.position.y;
        }
        Quaternion targetVec = Quaternion.LookRotation((TargetPosition - transform.position).normalized, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetVec, Speed * Time.deltaTime);
    }

    void StepEnd()
    {

        animator.SetFloat("WalkSpeed", 0);
        WalkPauseTimer = WalkPause + Random.Range(-WalkPauseRand, WalkPauseRand);
        if (NavMesh.CalculatePath(transform.position, GlobalGoal, NavMesh.AllAreas, path))
        {
            LocalGoal = path.corners[1];
        }
        else
        {
            Debug.LogError("Unable to path to target!");
            LocalGoal = GlobalGoal;
        }
    }

    void UpdateWalk(Vector3 targetLocation)
    {
        GlobalGoal = targetLocation;
        if (WalkPauseTimer > 0)
        {
            WalkPauseTimer -= Time.deltaTime;
            if (!NavMesh.Raycast(transform.position, targetLocation, out NavMeshHit hit, NavMesh.AllAreas)) //check a ray on the navmesh, if it doesn't hit anything we've got a straight shot to the target (eg if the player moved into sight since the last nav update)
            {
                LocalGoal = GlobalGoal = targetLocation;
            }
            TurnToFace(LocalGoal, TurnSpeed);
        }
        else
        {
            animator.SetFloat("WalkSpeed", WalkSpeed);
        }
        GlobalTools.SnapToGround(characterController, GroundSnapDistance);

        /*
        foreach (Vector3 point in path.corners)
        {
            Debug.DrawLine(point, point + new Vector3(0, 1, 0));
        }
        */
    }

    float PlayerLeash()
    {
        return (GlobalTools.player.transform.position - SpawnPosition).magnitude;
    }
}
