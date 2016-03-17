using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks.Movement;
using BehaviorDesigner.Runtime;
using Pathfinding;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Patrol around the specified waypoints using A* Pathfinding Project and AILerp for 2D Movement")]
[TaskCategory("Custom ")]
public class TwoDPatrol : CustomAIPathMovementScript
{

    
    public SharedBool randomPatrol = false;
    
    public SharedFloat waypointPauseDuration = 0;
    public SharedInt wayPointCounter = 0;
    
    public SharedGameObjectList waypoints;
    public SharedBool Alerted;
    // The current index that we are heading towards within the waypoints array
    private int waypointIndex;
    private float waypointReachedTime;

    public override void OnStart()
    {
        base.OnStart();

        // initially move towards the closest waypoint
        float distance = Mathf.Infinity;
        float localDistance;
        if (wayPointCounter.Value == -1)
        {
            for (int i = 0; i < waypoints.Value.Count; ++i)
            {
                if ((localDistance = Vector3.Magnitude(transform.position - waypoints.Value[i].transform.position)) < distance)
                {
                    distance = localDistance;
                    waypointIndex = i;
                }
            }
        }
        else
        {
            waypointIndex = wayPointCounter.Value;
        }
        waypointReachedTime = -waypointPauseDuration.Value;
        SetDestination(Target());
    }

    // Patrol around the different waypoints specified in the waypoint array. Always return a task status of running. 
    public override TaskStatus OnUpdate()
    {
        if (Alerted.Value)
        {
            return TaskStatus.Failure;
        }
        if (HasArrived())
        {

            if (waypointReachedTime == -waypointPauseDuration.Value)
            {
                waypointReachedTime = Time.time;
            }
            // wait the required duration before switching waypoints.
            if (waypointReachedTime + waypointPauseDuration.Value <= Time.time)
            {
                if (randomPatrol.Value)
                {
                    if (waypoints.Value.Count == 1)
                    {
                        waypointIndex = 0;
                    }
                    else
                    {
                        // prevent the same waypoint from being selected
                        var newWaypointIndex = waypointIndex;
                        while (newWaypointIndex == waypointIndex)
                        {
                            newWaypointIndex = Random.Range(0, waypoints.Value.Count - 1);
                        }
                        waypointIndex = newWaypointIndex;
                    }
                }
                else
                {
                    waypointIndex = (waypointIndex + 1) % waypoints.Value.Count;
                }
                SetDestination(Target());
                waypointReachedTime = -waypointPauseDuration.Value;
                wayPointCounter.Value = waypointIndex;
                return TaskStatus.Success;
            }
        }

        return TaskStatus.Running;
    }

    // Return the current waypoint index position
    private Vector3 Target()
    {
        if (waypointIndex >= waypoints.Value.Count)
        {
            return transform.position;
        }
        return waypoints.Value[waypointIndex].transform.position;
    }

    // Reset the public variables
    public override void OnReset()
    {
        base.OnReset();

        randomPatrol = false;
        waypointPauseDuration = 0;
        waypoints = null;
    }

    // Draw a gizmo indicating a patrol 
    public override void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (waypoints == null)
        {
            return;
        }
        var oldColor = UnityEditor.Handles.color;
        UnityEditor.Handles.color = Color.yellow;
        for (int i = 0; i < waypoints.Value.Count; ++i)
        {
            if (waypoints.Value[i] != null)
            {
                UnityEditor.Handles.SphereCap(0, waypoints.Value[i].transform.position, waypoints.Value[i].transform.rotation, 1);
            }
        }
        UnityEditor.Handles.color = oldColor;
#endif
    }
}

