using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerTriggerScript : MonoBehaviour {
    [AdvancedInspector.Inspect(AdvancedInspector.InspectorLevel.Debug)]
    public bool OnChair = false;
    public Slider CopySlider;
    [AdvancedInspector.Inspect(AdvancedInspector.InspectorLevel.Debug)]
    public Image FillSliderArea;
    IEnumerator copyRoutine;
    [AdvancedInspector.Inspect(AdvancedInspector.InspectorLevel.Debug)]
    Color FillAreaOriginalColor;
    [AdvancedInspector.Inspect]
    public Color FinalColor;

    public GameObject Pencil;

    public bool NearNerd = false;
    void Start()
    {
        copyRoutine = CopyTest_Coroutine();
        FillSliderArea = CopySlider.fillRect.GetComponent<Image>();
        FillAreaOriginalColor = FillSliderArea.color;
    }

    void OnEnable()
    {
        Pencil.SetActive(false);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Chair"))
        {
            OnChair = true;
            if(CopySlider.value >= 1.0f)
            {
                GameManager.instance.GameOver(true);
            }
        }
        else if(other.CompareTag("Teacher"))
        {
            GameManager.instance.GameOver(false);
            TeacherBehaviorManager.instance.CaughtPlayer();
        }
        else if(other.CompareTag("Nerd"))
        {
            //StartCoroutine(copyRoutine);
            //Pencil.SetActive(true);
            NearNerd = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Chair"))
        {
            OnChair = false;
        }
        else if(other.CompareTag("Nerd"))
        {
            NearNerd = false;
            //StopCoroutine(copyRoutine);
            //Pencil.SetActive(false);
        }
    }

    public void CopyTestButton()
    {
        CopyTestFunction(true);
    }

    public bool CopyTestFunction(bool copy)
    {
        if (!NearNerd)
            return false;
#if COPY_CONTINIOUS
        if(copy)
        {
            StartCoroutine(copyRoutine);
        }
        else
        {
            StopCoroutine(copyRoutine);
        }
        Pencil.SetActive(copy);
#else
        CopySlider.value += 1.0f * Time.deltaTime / 10.0f;
        FillSliderArea.color = Color.Lerp(FillAreaOriginalColor, FinalColor, CopySlider.value);
#endif
        return true;
    }

    IEnumerator CopyTest_Coroutine()
    {
        while(true)
        {
            CopySlider.value += 1.0f * Time.deltaTime / 10.0f;
            FillSliderArea.color = Color.Lerp(FillAreaOriginalColor, FinalColor, CopySlider.value);
            yield return null;
        }
    }
}
