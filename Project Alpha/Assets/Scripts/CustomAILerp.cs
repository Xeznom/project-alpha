using UnityEngine;
using System.Collections;

public class CustomAILerp : AILerp {

    public delegate void TargetReached();
    public static event TargetReached OnTargetReach;

    public override void OnTargetReached()
    {
        base.OnTargetReached();
        OnTargetReach();
    }
}
