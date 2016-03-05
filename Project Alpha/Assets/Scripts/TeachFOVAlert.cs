using UnityEngine;
using System.Collections;

public class TeachFOVAlert : MonoBehaviour {

    FOV2DEyes eyes;
    FOV2DVisionCone visionCone;
    TeacherBehaviorManager manager;
    float speed = -5;

    void Start()
    {
        eyes = GetComponentInChildren<FOV2DEyes>();
        visionCone = GetComponentInChildren<FOV2DVisionCone>();
        manager = TeacherBehaviorManager.instance;
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    void OnChangeFOVState(FOV2DVisionCone.Status newStatus)
    {
        switch (newStatus)
        {
            case FOV2DVisionCone.Status.Alert:
                manager.FoundPlayer();
                break;
            case FOV2DVisionCone.Status.Idle:
                break;
            case FOV2DVisionCone.Status.Suspicious:
                break;
            default:
                break;
        }

        visionCone.status = newStatus;
    }

    void Update()
    {
        for(int i = 0; i < eyes.hits2D.Count; i++)
        {
            if(eyes.hits2D[i].transform && eyes.hits2D[i].transform.CompareTag("Player"))
            {
                OnChangeFOVState(FOV2DVisionCone.Status.Alert);
                break;
            }
        }
    }


}
