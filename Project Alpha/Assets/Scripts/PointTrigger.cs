using UnityEngine;
using System.Collections;

public class PointTrigger : MonoBehaviour {
    string Player = "Player";
    void OnTriggerEnter(Collider collider)
    {
        Transform root = collider.transform.root;
        if (root.CompareTag(Player))
        {
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }
}
