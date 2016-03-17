using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using AdvancedInspector;

public class DetermineLevels : MonoBehaviour {
    public Sprite Circle;
    public Sprite MarkedOut;

    public int AmountOfLevel = 8;
    [Inspect(InspectorLevel.Debug)]
    GameObject[] TheButtons;
    [Inspect(InspectorLevel.Debug)]
    GameObject[] GridList;

    [Inspect]
	void Start () {
        GridList = GameObject.FindGameObjectsWithTag("GridLayout");
        System.Array.Sort(GridList, CompareObNames);
        TheButtons = new GameObject[AmountOfLevel * GridList.Length];
        for (int i = 0; i < GridList.Length;i++ )
        {
            for(int b = 0; b < AmountOfLevel; b++)
            {
                TheButtons[i * AmountOfLevel + b] = GridList[i].transform.GetChild(b).gameObject;
            }
        }

        for (int world = 0; world < TheButtons.Length / AmountOfLevel; world++)
        {
            for (int level = 0; level < AmountOfLevel; level++)
            {
                if (PlayerPrefs.GetInt("World_" + (world + 1).ToString() + "_" + (level + 1).ToString()) == 1)
                {
                    //I can select
                    TheButtons[world * AmountOfLevel + level].GetComponent<Image>().sprite = Circle;
                    TheButtons[world * AmountOfLevel + level].GetComponent<Button>().interactable = true;
                }
                else
                {
                    TheButtons[world * AmountOfLevel + level].GetComponent<Image>().sprite = MarkedOut;
                    TheButtons[world * AmountOfLevel + level].GetComponent<Button>().interactable = false;
                    //Cannot select
                }
            }
        }
        PlayerPrefs.SetInt("World_1_1", 1);
        TheButtons[0].GetComponent<Image>().sprite = Circle;
	}

    [Inspect]
    void ClearArrays()
    {
        TheButtons = null;
        GridList = null;
    }

    int CompareObNames(GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }
}
