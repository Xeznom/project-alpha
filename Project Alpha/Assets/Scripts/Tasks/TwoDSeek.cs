using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks.Movement;
using BehaviorDesigner.Runtime;
using Pathfinding;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Seek Object with A* Pathfinding Project and AILerp for 2D Movement")]
[TaskCategory("Custom")]

public class TwoDSeek : CustomAIPathMovementScript
{
    public SharedGameObject target;
    public SharedVector3 targetPosition;

    public override void OnStart()
    {
        base.OnStart();

        SetDestination(Target());
    }

    // Seek the destination. Return success once the agent has reached the destination.
    // Return running if the agent hasn't reached the destination yet
    public override TaskStatus OnUpdate()
    {
        if (HasArrived())
        {
            return TaskStatus.Success;
        }

        SetDestination(Target());

        return TaskStatus.Running;
    }

    // Return targetPosition if target is null
    private Vector3 Target()
    {
        if (target.Value != null)
        {
            return target.Value.transform.position;
        }
        return targetPosition.Value;
    }

    public override void OnReset()
    {
        base.OnReset();
        target = null;
        targetPosition = Vector3.zero;
    }
}
