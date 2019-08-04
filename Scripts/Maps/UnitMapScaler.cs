using Assets.CommonLibrary.GenericClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitMapScaler : MonoBehaviour
{
    public bool LocalScale = true;
    // Start is called before the first frame update
    void Start()
    {
        WindowManager.ScreenSizeChangeEvent += UpdateScale;
        StartCoroutine(UpdateScale());
    }

    public IEnumerator UpdateScale()
    {
        yield return new WaitUntil(() => GameSceneContext.Map != null && WindowManager.LastScreenSize != default(Vector2Int));
        UpdateScale(WindowManager.LastScreenSize.x, WindowManager.LastScreenSize.y);
    }
    public void UpdateScale(int Width, int Height)
    {
        if (GameSceneContext.Map != null)
        {
            var parent = GetComponentInParent<Canvas>();
            if (parent != null)
                transform.localScale = (GameSceneContext.Map.Size 

                    );
            else
            {
                var parentSize = LocalScale ? (Vector2)transform.parent.localScale : new Vector2(Width, Height);
                transform.localScale = Vector3.one;
                transform.localScale = GameSceneContext.Map.Size.Divide(transform.lossyScale);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
