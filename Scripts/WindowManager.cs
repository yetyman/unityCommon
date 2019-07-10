using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour {
    public delegate void ScreenSizeChangeEventHandler(int Width, int Height);       //  Define a delgate for the event
    public static event ScreenSizeChangeEventHandler ScreenSizeChangeEvent;                //  Define the Event
    protected virtual void OnScreenSizeChange(int Width, int Height)
    {              //  Define Function trigger and protect the event for not null;
        ScreenSizeChangeEvent?.Invoke(Width, Height);
    }
    private static Vector2Int lastScreenSize;
    public static WindowManager instance = null;                                    //  Singleton for call just one instance

    public static Vector2Int LastScreenSize
    {
        get
        {
            return lastScreenSize;
        }
    }

    void Awake()
    {
        lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        instance = this;                                                            // Singleton instance
    }
    void Update()
    {
        Vector2Int screenSize = new Vector2Int(Screen.width, Screen.height);
        if (lastScreenSize != screenSize)
        {
            lastScreenSize = screenSize;
            OnScreenSizeChange(Screen.width, Screen.height);                        //  Launch the event when the screen size change
        }
    }
}
