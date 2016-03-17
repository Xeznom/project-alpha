using UnityEngine;
using System.Collections;
using AdvancedInspector;
using Lean;
using DG.Tweening;
public class SlidingMenu : MonoBehaviour {
    public RectTransform DefaultRect;
    public RectTransform ContentParent;
    public RectTransform[] Content;

    public float margin;

    public float MoveTime = 1f;

    [Inspect(InspectorLevel.Debug)]
    private float Step = 0;
    [Inspect(InspectorLevel.Debug)]
    private Vector3 DisplayPosition;
    [Inspect(InspectorLevel.Debug)]
    private float InternalCounter = 0;

    WaitForEndOfFrame EndFrame;
    Sequence seq;
    // Use this for initialization
    void Start()
    {
        GenerateContentParent();
        EndFrame = new WaitForEndOfFrame();
        //seq = DOTween.Sequence();
	}

    void OnEnable()
    {
        LeanTouch.OnFingerSwipe += OnFingerSwipe;
    }

    void OnDisable()
    {
        LeanTouch.OnFingerSwipe -= OnFingerSwipe;
    }
	
    [Inspect]
    void GenerateContentParent()
    {
        float Width = DefaultRect.rect.width;
        Width *= Content.Length;
        ContentParent.rect.Set(0,0, Width, ContentParent.rect.y);

        float newWdith = DefaultRect.localPosition.x;
        for (int i = 0; i < Content.Length; i++, newWdith += DefaultRect.rect.width + margin)
        {
            Content[i].pivot = new Vector2(0f, 0.5f);
            Content[i].localPosition = new Vector2(newWdith, DefaultRect.localPosition.y);
        }
        Step = Mathf.Round(Width / Content.Length) + margin;
        DisplayPosition = DefaultRect.localPosition;
        InternalCounter = 0;

    }

    void OnFingerSwipe(LeanFinger finger)
    {
        var swipe = finger.SwipeDelta;

        if (swipe.x < -Mathf.Abs(swipe.y))
        {
            MoveLeft();
        }

        else if (swipe.x > Mathf.Abs(swipe.y))
        {
            MoveRight();
        }
    }

    public void Button(bool back)
    {
        if(back)
        {
            //Left
            MoveLeft();
        }
        else
        {
            //Right
            MoveRight();
        }
    }

    void MoveLeft()
    {
        if (!DOTween.IsTweening(ContentParent))
        {
            //InfoText.text = "You swiped left!";
            //htings go right
            //Debug.Log("Seq is " + seq.IsPlaying());
            InternalCounter++;
            float newPosX = ContentParent.localPosition.x - Step;
            if (InternalCounter >= Content.Length)
            {
                InternalCounter--;
                if (seq == null || !seq.IsPlaying())
                {
                    seq = DOTween.Sequence();
                    seq.Append(ContentParent.DOLocalMoveX(-Step / 2, MoveTime).SetRelative(true));
                    seq.Append(ContentParent.DOLocalMoveX(ContentParent.localPosition.x, MoveTime)).SetAutoKill(true);
                }
            }
            else
            {
                ContentParent.DOLocalMoveX(newPosX, MoveTime).SetEase(Ease.InQuad);
            }
        }
    }

    void MoveRight()
    {
        if (!DOTween.IsTweening(ContentParent))
        {
            //InfoText.text = "You swiped right!";
            InternalCounter--;
            float newPosX = ContentParent.localPosition.x + Step;
            if (InternalCounter < 0)
            {
                InternalCounter++;
                if (seq == null || !seq.IsPlaying())
                {
                    seq = DOTween.Sequence();
                    seq.Append(ContentParent.DOLocalMoveX(Step / 2, MoveTime).SetRelative(true));
                    seq.Append(ContentParent.DOLocalMoveX(ContentParent.localPosition.x, MoveTime)).SetAutoKill(true);
                }
            }
            else
            {
                ContentParent.DOLocalMoveX(newPosX, MoveTime).SetEase(Ease.OutQuad);
            }
        }
    }
}
