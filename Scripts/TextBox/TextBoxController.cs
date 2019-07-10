using Assets.Scripts.Animation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TextBox
{
    public partial class TextBoxController : MonoBehaviour
    {

        public string ControllerName = "Main";
        public Text TitleTxt = null;
        public Text BodyTxt = null;
        public SpriteRenderer sr = null;
        public SpriteScaler ss = null;
        bool clickable = false;


        
        public bool IsClickable
        {
            get
            {
                return clickable;
            }

            set
            {
                clickable = value;
            }
        }

        // Use this for initialization
        void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            ss = GetComponent<SpriteScaler>();

            if (GameSceneContext.TextBoxes.ContainsValue(this))
            {
                ControllerName = GameSceneContext.TextBoxes.FirstOrDefault(x => x.Value == this).Key;
            }else
                GameSceneContext.TextBoxes.Add(ControllerName, this);
            SetUpAnimations();
        }

        // Update is called once per frame
        void Update()
        {
        }

        internal void SetDialogue(string text, string sName, string displayName, string displayBox)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            if (TitleTxt != null)
            {
                if (!string.IsNullOrEmpty(displayName))
                    TitleTxt.text = displayName;
                else
                    TitleTxt.text = sName;
            }

            if (BodyTxt != null)
                BodyTxt.text = text;

            if (!string.IsNullOrEmpty(displayBox) && GameSceneContext.BGs.ContainsKey(displayBox) && GameSceneContext.BGs[displayBox] != sr.sprite)
                sr.sprite = GameSceneContext.BGs[displayBox];
        }
        public void SetClickable(bool clickable)
        {
            this.clickable = clickable;
        }

        public void ContinueConvo()
        {
            if(GameSceneContext.ScreenPlay!=null)
                GameSceneContext.ScreenPlay.ContinueConvo(ControllerName);
        }
        //public void MoveTo(Rect destination, float duration)
        //{
        //    //TODO: all this
        //    //minimize this box, take duration * 1/2

        //    //expand transition panel, start 1/4 in, duration 1/4
        //    //minimize transition panel, start 1/2 in, duration 1/4

        //    //maximize new box, start 1/2 in, duration 1/2
        //}


        
    }
}