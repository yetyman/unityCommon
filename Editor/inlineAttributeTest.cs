using Assets.CustomAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inlineAttributeTest : MonoBehaviour
{
    public bool hi = false;
    public bool there = false;
    [BeginHorizontal]
    [BeginVertical]
    public bool hi2;
    public string hi3;
    [EndVertical]
    public string hi5;
    [BeginVertical]
    public bool hi6;
    public string hi7;
    public bool hi8;
    [EndVertical]
    [EndHorizontal]
    public string there2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
