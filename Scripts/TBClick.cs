using Assets.Scripts.TextBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TBClick : MonoBehaviour {

    public TextBoxController parent = null;
	// Use this for initialization
	void Start () {
        if(parent == null)
            parent = GetComponentInParent<TextBoxController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnMouseDown()
    //private void OnPointerDown(PointerEventData eventData)
    {
        if (parent.IsClickable)
            parent.ContinueConvo();
    }
}
