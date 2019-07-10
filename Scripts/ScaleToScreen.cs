using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleToScreen : MonoBehaviour
{
    public bool Center = true;
    // Start is called before the first frame update
    void Start()
    {
        WindowManager.ScreenSizeChangeEvent += WindowManager_ScreenSizeChangeEvent;
    }

    private void WindowManager_ScreenSizeChangeEvent(int Width, int Height)
    {
        transform.localScale = new Vector3(Width, Height, 1);
        transform.localPosition = new Vector3 (transform.localScale.x * -.5f, transform.localScale.y * -.5f,1);
    }

}
