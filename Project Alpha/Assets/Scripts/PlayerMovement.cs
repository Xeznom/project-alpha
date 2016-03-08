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


    CustomAILerp PlayerAI;
    Seeker PlayerSeeker;
    Transform PlayerTransform;
    Transform currentPoint;
    [AdvancedInspector.Inspect(InspectorLevel.Debug)]
    Transform currentMovingPoint;

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
        PlayerSeeker = GetComponent<Seeker>();
        PlayerTransform = transform;

    }

    protected virtual void OnEnable()
    {
        LeanTouch.OnFingerSet += OnFingerDown;
        LeanTouch.OnFingerUp += OnFingerUp;
        CustomAILerp.OnTargetReach += OnTargetReach;

    }

    protected virtual void OnDisable()
    {
        LeanTouch.OnFingerSet -= OnFingerDown;
        LeanTouch.OnFingerUp -= OnFingerUp;
        CustomAILerp.OnTargetReach -= OnTargetReach;

        RegisteredPoints.Clear();
        ListOfPoints.Clear();
        currentMovingPoint = null;
    }

    void OnFingerDown(LeanFinger finger)
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
        Vector3 fingerPos = finger.GetWorldPosition(transform.position.z);
        Vector3 WorldPos = new Vector3(fingerPos.x, fingerPos.y, 0);
        var ray = finger.GetRay();
        var hit = default(RaycastHit);

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
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
        dummyTargetPoint.position = notWaypointPoint.position;
        PlayerAI.SearchPath();
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

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("On trigger enter");
        if(other.CompareTag("Teacher"))
        {
            Debug.Log("Game OVER");
            //LeanPool.Despawn(this.gameObject);
        }
    }
}
