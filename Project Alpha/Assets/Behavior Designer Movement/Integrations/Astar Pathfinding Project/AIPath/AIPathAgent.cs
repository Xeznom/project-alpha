using UnityEngine;

namespace Pathfinding
{
    // Subclass the AIPath class to provide common functions for the Movement agents
    public class AIPathAgent : AIPath
    {
        // Has the path been created and is valid?
        public bool PathCalculated()
        {
            return path != null && !path.error;
        }

        // Set the path to null so PathCalculated will return false
        public void RemovePath()
        {
            path = null;
        }

        // Returns the agent's current velocity
        public Vector3 Velocity()
        {
            return CalculateVelocity(GetFeetPosition());
        }
    }
}