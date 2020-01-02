using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GlitchMonster : MonoBehaviour
{
    public float DetectRadius = 10;
    public float WalkSpeed = 1;
    [Tooltip("Pauses happen after every step, whereupon pathfinding updates- turning can only happen while pausing")]
    public float WalkPauseMin = .25f;
    [Tooltip("random amount of time added to the walk pause")]
    public float WalkPauseRand = .25f;
    public float TurnSpeed = 100;
    [Tooltip("The object that defines the territory the monster is allowed to attack the player within")]
    public AITerritory territory;
    [Tooltip("If there's no territory set, the AI is tethered this distance from its spawn point")]
    public float DefaultLeashLength = 10;
    public float WakeTime = 1;//how long it takes to activate after detecting player
    public float WakeTurnSpeed = .3f;
    public float HomeThreshold = .01f;
    public float GroundSnapDistance = 1;
    public AudioSource AudioAwake;
    float AudioAwakeMaxVolume;
    public AudioSource AudioClose;
    float AudioCloseMaxVolume;
    public float SafeDistance = 15;
    public HueAggression hueAggression;

    SphereCollider detectPlayer;

    public enum AIState { Idle, Activating, FollowingPlayer, FacingHome, ReturningHome, FacingIdle}
    [HideInInspector]
    public AIState aiState;

    CharacterController characterController;
    Animator animator;
    [HideInInspector]
    public float ActivateTimer;
    float WalkPauseTimer;
    Vector3 SpawnPosition;
    Quaternion SpawnRotation;
    NavMeshPath path;
    Vector3 LocalGoal;
    Vector3 GlobalGoal;
    
    // not used yet, will eventually be used so pathing isn't recalculated every step for returning home
    //bool MovingGoal = true;
    //this will involve first coming up with some sort of function to remove locations from path when they're passed, though
    //maybe check if path[1] is within an arc behind us? That would imply we've stepped past it

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
        AudioAwakeMaxVolume = AudioAwake.volume;
        AudioCloseMaxVolume = AudioClose.volume;
    }
    void OnTriggerStay(Collider other)
    {

        //no longer checking if other is player??
        if (other.gameObject == GlobalTools.player)
        {
            if ((aiState != AIState.FollowingPlayer && aiState != AIState.Activating) && PlayerInTerritory())
            {
                RaycastHit hit;
                Ray ray = new Ray(transform.position, (GlobalTools.player.transform.position - transform.position));
                if (Physics.Raycast(ray, out hit, DetectRadius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    Debug.DrawLine(transform.position, hit.point);
                    if (hit.collider.gameObject == GlobalTools.player)
                    {
                        ChangeAIState(AIState.Activating);
                    }
                }

            }

        }

    }
    void Update()
    {
        if (aiState == AIState.Activating)
        {
            float LeashCurrent = (transform.position - SpawnPosition).magnitude;
            if (!PlayerInTerritory())
            {
                if (LeashCurrent <= HomeThreshold) ChangeAIState(AIState.FacingIdle);
                else ChangeAIState(aiState = AIState.FacingHome);
            }
            ActivateTimer -= Time.deltaTime;
            TurnToFace(GlobalTools.player.transform.position, WakeTurnSpeed);
            if (ActivateTimer <= 0) ChangeAIState(AIState.FollowingPlayer);
            AudioAwake.volume = (1 - ActivateTimer / WakeTime) * AudioAwakeMaxVolume;
            AudioClose.volume = (1 - ActivateTimer / WakeTime) * PlayerDistance() * AudioCloseMaxVolume;
        }
        else if (aiState == AIState.FollowingPlayer)
        {
            float LeashCurrent = (transform.position - SpawnPosition).magnitude;
            if (!PlayerInTerritory())
            {
                if (LeashCurrent <= HomeThreshold) ChangeAIState(AIState.FacingIdle);
                else ChangeAIState(aiState = AIState.FacingHome);
            }
            else
            {
                UpdateWalk(GlobalTools.player.transform.position);
            }
            AudioClose.volume = PlayerDistance() * AudioCloseMaxVolume;
        }
        else if (aiState == AIState.FacingHome)
        {
            if (ActivateTimer > 0)
            {
                ActivateTimer -= Time.deltaTime;
                TurnToFace(SpawnPosition, WakeTurnSpeed);
                AudioAwake.volume = (ActivateTimer / WakeTime) * AudioAwakeMaxVolume;
                AudioClose.volume = (ActivateTimer / WakeTime) * PlayerDistance() * AudioCloseMaxVolume;
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

            float AudioOffset = Random.Range(0.0f, 1.0f);
            AudioAwake.time = AudioOffset * AudioAwake.clip.length;
            AudioClose.time = AudioOffset * AudioAwake.clip.length;

            AudioAwake.volume = 0;
            AudioClose.volume = 0;
            AudioAwake.Play();
            AudioClose.Play();
        }
        else if (newState == AIState.FollowingPlayer)
        {
            AudioAwake.volume = AudioAwakeMaxVolume;
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
            AudioAwake.Stop();
            AudioClose.Stop();
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
        WalkPauseTimer = WalkPauseMin + Random.Range(0, WalkPauseRand);
        if (NavMesh.CalculatePath(transform.position + new Vector3(0, -characterController.height/2.1f, 0), GlobalGoal, NavMesh.AllAreas, path))
        {
            LocalGoal = path.corners[1];
        }
        else
        {
            Debug.LogError("Unable to path to target!", gameObject);
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
                //LocalGoal = GlobalGoal = targetLocation;
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
        for(int i = 1; i < path.corners.Length; i++)
        {
            Debug.DrawLine(path.corners[i-1], path.corners[i]);
        }
    }

    float PlayerLeash()
    {
        return (GlobalTools.player.transform.position - SpawnPosition).magnitude;
    }
    bool PlayerInTerritory()
    {
        if (territory)
        {
            return territory.PlayerInTerritory;
        }
        else if (PlayerLeash() < DefaultLeashLength)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    float PlayerDistance()
    {
        return 1 - Mathf.Clamp01(Vector3.Distance(transform.position, GlobalTools.player.transform.position) / SafeDistance);
    }

    public void Footstep()
    {
        hueAggression.StepFlash();
    }
}
