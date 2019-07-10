using Assets.Scripts;
using Assets.Scripts.Animation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public GameObject ProgressObj;
    public GameObject Filler;
    public GameObject MidSection;
    //public GameObject StartSection;
    //public GameObject EndSection;
    public float VisualProgress = 0;
    public float Progress = 0;
    public float TransitionTime = 1;
    private float LastProgressValue = .5f;
    private AnimationInstanceFloat<ProgressBar> ProgressAnimator;
    // Start is called before the first frame update
    void Start()
    {
        ProgressAnimator = new AnimationInstanceFloat<ProgressBar>(this, SetProgressVisual, GetProgressVisual, TransitionTime, EasingCurves.Sine);
        ProgressAnimator.Beginning += StartingToMove;
        ProgressAnimator.Ended += MovementEnding;
    }

    void StartingToMove(IPlayable p)
    {
        //start sparking effect. only override if current animation is ending animation....
    }
    void MovementEnding(IPlayable p)
    {
        //stop sparking effect if not cancelled
    }

    // Update is called once per frame
    void Update()
    {
        if(Progress != LastProgressValue)
        {
            SetProgress(Progress);
        }
    }

    public void SetProgress(float progress)
    {
        LastProgressValue = progress;
        ProgressAnimator.CancelImmediately();
        ProgressAnimator.Start(progress, null, TransitionTime);

    }
    public static float GetProgressVisual(ProgressBar pb)
    {
        return pb.VisualProgress;
    }
    float DeltaProgress = 0f;
    public static void SetProgressVisual(ProgressBar pb, float animatedProgressValue)
    {
        pb.DeltaProgress = animatedProgressValue - pb.VisualProgress;
        pb.VisualProgress = animatedProgressValue;
        pb.ProgressObj.transform.localPosition = new Vector3(animatedProgressValue/2, 0,0);
        pb.ProgressObj.transform.localScale = new Vector3(animatedProgressValue, 1,1);
        pb.Filler.transform.localPosition = new Vector3(1-(1-animatedProgressValue)/2, 0,0);
        pb.Filler.transform.localScale = new Vector3((1-animatedProgressValue), 1,1);
        pb.MidSection.transform.localPosition = new Vector3(animatedProgressValue, 0, 0);

        //pb.MidSection.transform.localScale = Vector3.one;
        //var lossyX = pb.MidSection.transform.lossyScale.x;
        //if (lossyX != 0)
        //    pb.MidSection.transform.localScale = new Vector3(1 / lossyX, 1, 1);
        //pb.MidSection.transform.localScale = new Vector3(1 / lossyX, 1, 1);
        //pb.MidSection.transform.localScale = new Vector3(1 / lossyX, 1, 1);
    }

}
