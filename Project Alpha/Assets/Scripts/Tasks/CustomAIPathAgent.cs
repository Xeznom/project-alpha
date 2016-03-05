using UnityEngine;
using System.Collections;

public class CustomAIPathAgent : AILerp
{
    public bool PathCalculated()
    {
        return path != null && !path.error;
    }

    // Set the path to null so PathCalculated will return false
    public void RemovePath()
    {
        path = null;
    }
}
