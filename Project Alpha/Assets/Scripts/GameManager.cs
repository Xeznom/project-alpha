using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AdvancedInspector;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [Inspect,Group("End Game UI")]
    public GameObject EndGameCanvas;
    [Inspect(InspectorLevel.Debug), Group("End Game UI")]
    private Image GradeImage;
    [Inspect, Group("End Game UI")]
    public Sprite AGrade;
    [Inspect, Group("End Game UI")]
    public Sprite BGrade;
    [Inspect, Group("End Game UI")]
    public Sprite CGrade;
    [Inspect, Group("End Game UI")]
    public Sprite DGrade;
    [Inspect, Group("End Game UI")]
    public Sprite FGrade;

    [Inspect, Group("Game Running UI")]
    public GameObject GameRunning;
    [Inspect, Group("Game Running UI")]
    public Slider Slide;

    public int World;
    public int WorldLevel;

    void Start()
    {
        instance = this;
        EndGameCanvas.SetActive(false);
        GradeImage = EndGameCanvas.GetComponentInChildren<Image>();

    }

    void OnDestroy()
    {
        instance = null;
    }

    public void GameOver(bool win)
    {
        Sprite FinalSpriteUsed = FGrade;
        if(win)
        {
            float grades = DetermineGrade();
            if(grades >= 1.0)
            {
                FinalSpriteUsed = AGrade;
            }
            else if(grades >= 0.8f)
            {
                FinalSpriteUsed = BGrade;
            }
            else if(grades >= 0.7f)
            {
                FinalSpriteUsed = CGrade;
            }
        }
        GradeImage.sprite = FinalSpriteUsed;
        GameRunning.SetActive(false);
        EndGameCanvas.SetActive(true);

        //Set next world level to be able to play
        PlayerPrefs.SetInt("World_" + World.ToString() + "_" + (WorldLevel + 1).ToString(), 1);
    }

    float DetermineGrade()
    {
        return Slide.value;
    }


}
