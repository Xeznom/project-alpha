using UnityEngine;
using System.Collections;
using Pathfinding;
using BehaviorDesigner.Runtime.Tasks.Movement;
using System;
using BehaviorDesigner.Runtime;
public abstract class CustomAIPathMovementScript : Movement {
    [Tooltip("The speed of the agent")]
    public SharedFloat speed = 3;
    [Tooltip("Turn speed of the agent")]
    public SharedFloat turningSpeed = 5;

    // A cache of the AIPath
    private CustomAIPathAgent aiPathAgent;

    public override void OnAwake()
    {
        // cache for quick lookup
        aiPathAgent = gameObject.GetComponent<CustomAIPathAgent>();

        if (aiPathAgent.target == null)
        {
            var target = new GameObject();
            target.name = Owner.name + " target";
            aiPathAgent.target = target.transform;
        }
    }

    public override void OnStart()
    {
        // set the speed and turning speed, then enable the agent
        aiPathAgent.speed = speed.Value;
        aiPathAgent.rotationSpeed = turningSpeed.Value;
        aiPathAgent.RemovePath();
        aiPathAgent.SearchPath();
        aiPathAgent.enabled = true;
    }

    protected override bool SetDestination(Vector3 target)
    {
        if (aiPathAgent.target.position != target)
        {
            aiPathAgent.target.position = target;
        }
        return true;
    }

    protected override bool HasArrived()
    {
        return aiPathAgent.PathCalculated() && aiPathAgent.targetReached;
    }

    public override void OnEnd()
    {
        // Disable the AIPath
        aiPathAgent.enabled = false;
    }

    // Reset the public variables
    public override void OnReset()
    {
        speed = 3;
        turningSpeed = 5;
    }

    protected override Vector3 Velocity()
    {
        return Vector3.zero;
    }
}
