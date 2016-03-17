using UnityEngine;
using System.Collections;
using AdvancedInspector;
using Lean;
using System.Collections.Generic;
using Pathfinding;
public class PlayerMovement : MonoBehaviour
{ 
    
    private Queue<Transform> ListOfPoints;
    private List<Transform> RegisteredPoints;

    public LineRenderer lineRender;

    public GameObject PathPointPrefab;
    public float AgentThreshold = 1.0f;

    public Transform dummyTargetPoint;

    PlayerTriggerScript playerTriggers;


    CustomAILerp PlayerAI;

    bool StartCopyingNotes;
    bool StillCopyingNotes;

#if WAYPOINT 
    Seeker PlayerSeeker;
    Transform PlayerTransform;
    Transform currentMovingPoint;
#endif

    Transform currentPoint;
    [AdvancedInspector.Inspect(InspectorLevel.Debug)]


#if !WAYPOINT
    Transform notWaypointPoint;
#endif

    // Use this for initialization
    void Start()
    {
        lineRender = GetComponent<LineRenderer>();
        RegisteredPoints = new List<Transform>();
        ListOfPoints = new Queue<Transform>();
        PlayerAI = GetComponent<CustomAILerp>();
        playerTriggers = GetComponent<PlayerTriggerScript>();
#if WAYPOINT
        PlayerSeeker = GetComponent<Seeker>();
        PlayerTransform = transform;
#endif

    }

    protected virtual void OnEnable()
    {
        LeanTouch.OnFingerDown += OnFingerDown;
        LeanTouch.OnFingerSet += OnFingerSet;
        LeanTouch.OnFingerUp += OnFingerUp;
        CustomAILerp.OnTargetReach += OnTargetReach;

    }

    protected virtual void OnDisable()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerSet -= OnFingerSet;
        LeanTouch.OnFingerUp -= OnFingerUp;
        CustomAILerp.OnTargetReach -= OnTargetReach;

        RegisteredPoints.Clear();
        ListOfPoints.Clear();
#if WAYPOINT
        currentMovingPoint = null;
#endif
    }

    void OnFingerDown(LeanFinger finger)
    {
        if(StartCopyingNotes)
        {
            Vector3 fingerPos = finger.GetWorldPosition(transform.position.z);

            RaycastHit2D hit2d = Physics2D.Raycast(fingerPos, Vector2.zero);
            if (hit2d == null || hit2d.collider == null || hit2d.collider.name != "Nerd")
            {
                StartCopyingNotes = false;
            }
            else
            {
                playerTriggers.CopyTestFunction(true);
                StillCopyingNotes = true;
            }
        }
    }

    void OnFingerSet(LeanFinger finger)
    {
#if WAYPOINT
        Vector3 fingerPos = finger.GetWorldPosition(transform.position.z);
        Vector3 WorldPos = new Vector3(fingerPos.x, fingerPos.y, 0);

        var ray = finger.GetRay();
        var hit = default(RaycastHit);

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
        {
            if (hit.collider.CompareTag("Node"))
            {
                currentPoint = hit.collider.transform;
            }
            else if (hit.collider.CompareTag("Ground"))
            {
                if (currentPoint == null)
                {
                    GameObject obj = Lean.LeanPool.Spawn(PathPointPrefab, Vector3.zero, Quaternion.identity);
                    currentPoint = obj.transform;
                }
            }
            if (currentPoint != null)
            {
                currentPoint.transform.position = WorldPos;
            }
            if(currentPoint == currentMovingPoint)
            {
                dummyTargetPoint.position = currentMovingPoint.position;
            }
        }
#else
        if (!StartCopyingNotes)
        {
            
            Vector3 fingerPos = finger.GetWorldPosition(transform.position.z);
            Vector3 WorldPos = new Vector3(fingerPos.x, fingerPos.y, 0);
            var ray = finger.GetRay();
            var hit = default(RaycastHit);

            RaycastHit2D[] hits2D = Physics2D.RaycastAll(fingerPos, Vector2.zero);
            for (int i = 0; i < hits2D.Length; i++)
            {
                //Hacky method. Not changeable.
                if (hits2D[i] != null && hits2D[i].collider != null && hits2D[i].collider.name == "Nerd" && playerTriggers.CopyTestFunction(true))
                {
                    StartCopyingNotes = true;
                    StillCopyingNotes = true;
                    break;
                }
            }

            if (!StartCopyingNotes && Physics.Raycast(ray, out hit, float.PositiveInfinity))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    if (notWaypointPoint == null)
                    {
                        GameObject obj = Lean.LeanPool.Spawn(PathPointPrefab, Vector3.zero, Quaternion.identity);
                        notWaypointPoint = obj.transform;
                    }
                    notWaypointPoint.position = WorldPos;
                }
            }
        }
        else if (StillCopyingNotes)
        {
            playerTriggers.CopyTestFunction(true);
        }
#endif
    }

    void OnFingerUp(LeanFinger finger)
    {
#if WAYPOINT
        if (currentPoint != null)
        {
            Vector3 fingerPos = finger.GetWorldPosition(transform.position.z);
            Vector3 WorldPos = new Vector3(fingerPos.x, fingerPos.y, 0);
            ListOfPoints.Enqueue(currentPoint);
            currentPoint = null;

            //if (currentMovingPoint == null)
            {
                currentMovingPoint = ListOfPoints.Dequeue();
            }
            if (currentMovingPoint != null)
            {
                dummyTargetPoint.position = currentMovingPoint.position;
                PlayerAI.SearchPath();
            }


        }
#else
        if (notWaypointPoint != null)
        {
            dummyTargetPoint.position = notWaypointPoint.position;
            PlayerAI.SearchPath();
        }
        StillCopyingNotes = false;
#endif
    }
    
    void OnTargetReach()
    {
#if WAYPOINT
        if (ListOfPoints.Count > 0)
        {
            if (currentPoint != currentMovingPoint)
            {
                if (currentMovingPoint.gameObject.activeSelf)
                {
                    LeanPool.Despawn(currentMovingPoint.gameObject);
                }
                currentMovingPoint = ListOfPoints.Dequeue();
            }

            if (currentMovingPoint == null)
                return;
            dummyTargetPoint.position = currentMovingPoint.position;
            PlayerAI.SearchPath();
        }
        else
        {
            if (currentPoint != currentMovingPoint)
            {
                if (currentMovingPoint.gameObject.activeSelf)
                {
                    LeanPool.Despawn(currentMovingPoint.gameObject);
                }
                currentMovingPoint = null;
            }
        }
#else
        if (notWaypointPoint != null && notWaypointPoint.gameObject.activeSelf)
        {
            LeanPool.Despawn(notWaypointPoint.gameObject);
            notWaypointPoint = null;
        }
#endif
    }
}
