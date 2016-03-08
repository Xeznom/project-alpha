using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

public class TeacherBehaviorManager : MonoBehaviour {

    [AdvancedInspector.Inspect(AdvancedInspector.InspectorLevel.Advanced)]
    public static TeacherBehaviorManager instance;
    private List<Behavior> patrolBehavior = new List<Behavior>();
    private List<Behavior> seekBehavior = new List<Behavior>();

    void Awake()
    {
        instance = this;
        
        var allBehaviors = FindObjectsOfType(typeof(Behavior)) as Behavior[];
        for (int i = 0; i < allBehaviors.Length; ++i)
        {
            if (allBehaviors[i].Group == 0)
            { // 0 indicates flag not taken behaviors
                patrolBehavior.Add(allBehaviors[i]);
            }
            else
            { // 1 indicates flag taken behaviors
                seekBehavior.Add(allBehaviors[i]);
                allBehaviors[i].DisableBehavior(true);
            }
        }
    }
    
    public void FoundPlayer()
    {

        for (int i = 0; i < seekBehavior.Count; i++)
        {
            patrolBehavior[i].DisableBehavior();
        }
        for(int i = 0; i < seekBehavior.Count; i++)
        {
            seekBehavior[i].EnableBehavior();
        }
    }
}
