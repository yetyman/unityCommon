using Assets.CommonLibrary.GenericClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteScaler : MonoBehaviour
{

    SpriteRenderer sr;
    RectTransform rt;

    private float leftRatio = 0;
    private float rightRatio = 0;
    private float topRatio = 0;
    private float bottomRatio = 0;

    [SerializeField, GetSet("StretchLeft")]
    private bool stretchLeft = false;
    [SerializeField, GetSet("StretchRight")]
    private bool stretchRight = false;
    [SerializeField, GetSet("StretchTop")]
    private bool stretchTop = false;
    [SerializeField, GetSet("StretchBottom")]
    private bool stretchBottom = false;

    //TODO: add aspect ratio option which disables all these
    
    public bool StretchLeft
    {
        get
        {
            return stretchLeft;
        }

        set
        {
            stretchLeft = value;
            if (stretchLeft && rt != null)
                leftRatio = rt.offsetMin.x / rt.TotalWidth();
        }
    }

    public bool StretchRight
    {
        get
        {
            return stretchRight;
        }

        set
        {
            stretchRight = value;
            if (stretchRight && rt != null)
                rightRatio = (-rt.offsetMax.x) / rt.TotalWidth();
        }
    }

    public bool StretchTop
    {
        get
        {
            return stretchTop;
        }

        set
        {
            stretchTop = value;
            if (stretchTop && rt != null)
                topRatio = (-rt.offsetMax.y) / rt.TotalHeight();
        }
    }

    public bool StretchBottom
    {
        get
        {
            return stretchBottom;
        }

        set
        {
            stretchBottom = value;
            if (stretchBottom && rt != null)
                bottomRatio = rt.offsetMin.y / rt.TotalHeight();
        }
    }

    // Use this for initialization
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rt = GetComponent<RectTransform>();
        
        var initialsize = rt.rect.size;
        initialsize.x /= rt.localScale.x;
        initialsize.y /= rt.localScale.y;
        sr.size = initialsize;

        StretchLeft = StretchLeft;
        StretchRight = StretchRight;
        StretchTop = StretchTop;
        StretchBottom = StretchBottom;
        WindowManager.ScreenSizeChangeEvent += Instance_ScreenSizeChangeEvent;
    }

    public void RefreshLayout()
    {
        //changed = true;//not doing calculation here to avoid bogging down the source's update. doing calculations here instead.

        var leftSize = (stretchLeft) ? leftRatio * rt.TotalWidth() : rt.offsetMin.x;
        var rightSize = (stretchRight) ? rightRatio * rt.TotalWidth() : -rt.offsetMax.x;
        var topSize = (stretchTop) ? topRatio * rt.TotalHeight() : -rt.offsetMax.y;
        var bottomSize = (stretchBottom) ? bottomRatio * rt.TotalHeight() : rt.offsetMin.y;
        rt.SetRect(leftSize, topSize, rightSize, bottomSize);

        Vector2 size = sr.size;
        size.x = rt.rect.width;
        size.y = rt.rect.height;

        //funfact thats hard to diagnose, you'll get errors if you set the sprite render's size to 0! ha. it sucks. so here.
        sr.size = size;
    }

    private void Instance_ScreenSizeChangeEvent(int Width, int Height)
    {
        RefreshLayout();
    }

    // Update is called once per frame
    void Update()
    {
        //if (changed)
        //{
        //    changed = false;
        //    //do math.
        //    //its in update on purpose.
        //    var leftSize = (stretchLeft) ? leftRatio * rt.TotalWidth() : rt.offsetMin.x;
        //    var rightSize = (stretchRight) ? rightRatio * rt.TotalWidth() : -rt.offsetMax.x;
        //    var topSize = (stretchTop) ? topRatio * rt.TotalHeight() : -rt.offsetMax.y;
        //    var bottomSize = (stretchBottom) ? bottomRatio * rt.TotalHeight() : rt.offsetMin.y;
        //    rt.SetRect(leftSize, topSize, rightSize, bottomSize);

        //    Vector2 size = sr.size;
        //    size.x = rt.rect.width / rt.localScale.x;
        //    size.y = rt.rect.height / rt.localScale.y;
        //    sr.size = size;
        //}
    }
}
