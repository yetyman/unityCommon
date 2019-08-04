using Assets.CommonLibrary.GenericClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class ButtonDictionary : SerializableDictionary<string, LongPressButton> { }


public delegate void ClickEvent(LongPressButton button);
public class LongPressButton : Button
{
    public ClickEvent onLongPress;
    // Use this for initialization
    new void Start()
    {
        base.Start();
        //TODO: time for long press events
    }
    // Update is called once per frame
    void Update()
    {

    }
    Coroutine showTooltip = null;
    public override void OnPointerDown(PointerEventData eventData)
    {
        showTooltip = StartCoroutine(ShowToolTip(.5f));
        base.OnPointerDown(eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (showTooltip != null)
            StopCoroutine(showTooltip);
        base.OnPointerUp(eventData);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (showTooltip != null)
            StopCoroutine(showTooltip);
        base.OnPointerExit(eventData);
    }
    IEnumerator ShowToolTip(float timeRequired)
    {
        yield return new WaitForSeconds(timeRequired);
        
        onLongPress.Invoke(this);
        // this is a long press or drag
    }
}
