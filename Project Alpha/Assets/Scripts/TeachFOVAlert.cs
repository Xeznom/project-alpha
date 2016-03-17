using UnityEngine;
using System.Collections;
using AdvancedInspector;
using BehaviorDesigner.Runtime;
using UnityEngine.UI;
public class TeachFOVAlert : MonoBehaviour {

    FOV2DEyes eyes;
    FOV2DVisionCone visionCone;
    TeacherBehaviorManager manager;
    [AdvancedInspector.Inspect(AdvancedInspector.InspectorLevel.Debug)]
    [RangeValue(0, 1)]
    float SuspicionBarPercentage;
    [AdvancedInspector.Inspect(AdvancedInspector.InspectorLevel.Debug)]
    float TimeLeft = 2.0f;
    [AdvancedInspector.Inspect(AdvancedInspector.InspectorLevel.Debug)]
    bool Alerted;

    BehaviorDesigner.Runtime.Behavior PatrolState;
    SharedBool alerted;

    public Image CanvasWorldAlertBar;

    public Image ExpresionImage;
    public Sprite[] Expressions;

    public AudioSource detect_sound;
    public Transform TargetLookAt;

    enum Expresions
    {
        Suspicios = 0,
        Alerted = 1
    }

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
        alerted = (SharedBool)PatrolState.GetVariable("Alerted");
    }

    void OnEnable()
    {
        ExpresionImage.sprite = null;
        ExpresionImage.color = new Color(0f, 0f, 0f, 0f);
        SuspicionBarPercentage = 0;
        CanvasWorldAlertBar.color = Color.green;
        ExpresionImage.sprite = null;
    }

    void OnDisable()
    {

    }

    float totalSuspicion = 2.0f;

    void OnChangeFOVState(FOV2DVisionCone.Status newStatus)
    {
        switch (newStatus)
        {
            case FOV2DVisionCone.Status.Alert:
                manager.FoundPlayer();
                ExpresionImage.sprite = Expressions[(int)Expresions.Alerted];
                ExpresionImage.color = Color.white;
                this.enabled = false;
                break;
            case FOV2DVisionCone.Status.Idle:

                if (TimeLeft <= 0)
                {
                    alerted.Value = false;
                    ExpresionImage.sprite = null;
                    ExpresionImage.color = new Color(0f, 0f, 0f, 0f);
                    PatrolState.EnableBehavior();
                }
                break;
            case FOV2DVisionCone.Status.Suspicious:
                if(!detect_sound.isPlaying)
                    detect_sound.Play();
                SuspicionBarPercentage += 1f * Time.deltaTime;
                float percentage = SuspicionBarPercentage / totalSuspicion;
                AffectGraphicsComponent(percentage);

                if (percentage >= 1.0f)
                {
                    OnChangeFOVState(FOV2DVisionCone.Status.Alert);
                }
                break;
            default:
                break;
        }

        visionCone.status = newStatus;
    }

    void AffectGraphicsComponent(float percentage)
    {
        visionCone.LerpToAlert(percentage);
        CanvasWorldAlertBar.color = Color.Lerp(Color.green, Color.red, percentage);
        CanvasWorldAlertBar.fillAmount = percentage;
        ExpresionImage.sprite = Expressions[(int)Expresions.Suspicios];
        ExpresionImage.color = Color.white;
    }

    void Update()
    {
        bool playerDetect = false;
        for(int i = 0; i < eyes.hits2D.Count; i++)
        {
            if(eyes.hits2D[i].transform && eyes.hits2D[i].transform.CompareTag("Player"))
            {
                if (!eyes.hits2D[i].transform.GetComponent<PlayerTriggerScript>().OnChair)
                {
                    TargetLookAt.position = eyes.hits2D[i].transform.position;
                    playerDetect = true;
                    alerted.Value = true;
                    TimeLeft = 2.0f;
                    OnChangeFOVState(FOV2DVisionCone.Status.Suspicious);
                }
                break;
            }
            else
            {
                OnChangeFOVState(FOV2DVisionCone.Status.Idle);
            }
        }
        if(!playerDetect)
        {
            detect_sound.Pause();
            if (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 LookAt = transform.forward * 10;
        Gizmos.DrawLine(transform.position, LookAt);
    }

    
}
