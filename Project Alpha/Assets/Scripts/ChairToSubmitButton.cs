using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ChairToSubmitButton : MonoBehaviour {

    public Button theButton;
    GameManager theManager;

    void Start()
    {
        theManager = GameManager.instance;
    }
    public void SetInteractable(bool set)
    {
        theButton.interactable = set;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (theManager == null)
            theManager = GameManager.instance;
        if (collider.CompareTag("Player"))
        {
            if(theManager.Slide.value > 0.5f)
                SetInteractable(true);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
            SetInteractable(false);
    }
}
