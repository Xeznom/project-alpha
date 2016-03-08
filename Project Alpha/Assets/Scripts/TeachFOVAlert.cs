using UnityEngine;
using System.Collections;
using AdvancedInspector;
using BehaviorDesigner.Runtime;
public class TeachFOVAlert : MonoBehaviour {

    FOV2DEyes eyes;
    FOV2DVisionCone visionCone;
    TeacherBehaviorManager manager;
    float speed = -5;
    [AdvancedInspector.Inspect(AdvancedInspector.InspectorLevel.Debug)]
    [RangeValue(0, 1)]
    float SuspicionBarPercentage;
    [AdvancedInspector.Inspect(AdvancedInspector.InspectorLevel.Debug)]
    float TimeLeft = 2.0f;

    BehaviorDesigner.Runtime.Behavior PatrolState;

    void Start()
    {
        eyes = GetComponentInChildren<FOV2DEyes>();
        visionCone = GetComponentInChildren<FOV2DVisionCone>();
        manager = TeacherBehaviorManager.instance;

        var allBehaviors = GetComponents<Behavior>();
        for (int i = 0; i < allBehaviors.Length; ++i)
        {
            if (allBehaviors[i].Group == 0)
            { // 0 indicates flag not taken behaviors
                PatrolState = allBehaviors[i];
                break;
            }
        }
    }

    void OnEnable()
    {
        SuspicionBarPercentage = 0;
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

                if (TimeLeft <= 0)
                {
                    PatrolState.EnableBehavior();
                }
                break;
            case FOV2DVisionCone.Status.Suspicious:
                PatrolState.DisableBehavior();
                SuspicionBarPercentage += 1f * Time.deltaTime;
                visionCone.LerpToAlert(SuspicionBarPercentage);
                if(SuspicionBarPercentage >= 2.0f)
                {
                    OnChangeFOVState(FOV2DVisionCone.Status.Alert);
                }
                break;
            default:
                break;
        }

        visionCone.status = newStatus;
    }

    void Update()
    {
        bool playerDetect = false;
        for(int i = 0; i < eyes.hits2D.Count; i++)
        {
            if(eyes.hits2D[i].transform && eyes.hits2D[i].transform.CompareTag("Player"))
            {
                playerDetect = true;
                TimeLeft = 2.0f;
                OnChangeFOVState(FOV2DVisionCone.Status.Suspicious);
                break;
            }
            else
            {
                OnChangeFOVState(FOV2DVisionCone.Status.Idle);
            }
        }
        if(!playerDetect)
        {
            TimeLeft -= Time.deltaTime;
        }
    }
}
