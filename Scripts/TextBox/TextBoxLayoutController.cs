using Assets.CommonLibrary.GenericClasses;
using Assets.Scripts.Animation;
using Assets.Scripts.StateMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.TextBox
{
    /// <summary>
    /// LayoutControls.
    /// </summary>
    public partial class TextBoxController
    {
        RectTransform rt;
        RectTransform TitleRt;
        RectTransform BodyRt;

        public string StartUpState;
        private StateMachine LayoutMachine;

        private bool visible = true;
        public float MoveTime = 1;
        public string Layout
        {
            get
            {
                return LayoutMachine.CurrentStateName;
            }
        }
        public bool Visible
        {
            get
            {
                return LayoutMachine.CurrentStateName != "Hidden";
            }
        }

        public void SetUpAnimations()
        {
            LayoutMachine = new StateMachine(this, new List<string>() { StartUpState }.ToList());

            rt = GetComponentInChildren<RectTransform>();
            BodyRt = BodyTxt.GetComponent<RectTransform>();
            TitleRt = TitleTxt.GetComponent<RectTransform>();

            LayoutMachine.SetTransition("Bubble", "Bubble", Direction.Advance);
            LayoutMachine.SetTransition("Hidden", "Dialogue", Direction.Advance);
            LayoutMachine.SetTransition("Hidden", "Bubble", Direction.Advance);
            LayoutMachine.SetTransition("Hidden", "Fullscreen", Direction.Advance);
            LayoutMachine.SetCyclesThrough(new[] { "Dialogue", "Bubble", "Fullscreen" }, Direction.Switch);

            //TODO: implement statemachine categories. make the names look like Bubble:Hidden or something. maybe add preference to same category state changes.
            LayoutMachine.SetTransition("Dialogue", "Hidden", Direction.Advance)
                .Beginning += (IPlayable p) =>
                {
                    rt.AnimateBounds(MoveTime,
                        topanchor: 0,
                        bottomanchor: -.3f,
                        curve: EasingCurves.Back,
                        pattern: EasingPatterns.In
                    );
                };
            LayoutMachine.SetTransition("Bubble", "Hidden", Direction.Advance)
                .Beginning += (IPlayable p) =>
                {
                    rt.AnimateBounds(MoveTime,
                        leftanchor: -.5f,
                        rightanchor: 0,
                        topanchor: BubbleLocation.y + .1f,
                        bottomanchor: BubbleLocation.y,
                        curve: EasingCurves.Back,
                        pattern: EasingPatterns.In
                    );
                };
            LayoutMachine.SetTransition("Fullscreen", "Hidden", Direction.Advance)
                .Beginning += (IPlayable p) =>
                {
                    rt.AnimateBounds(MoveTime,
                        topanchor: 0,
                        bottomanchor: -1f,
                        curve: EasingCurves.Bounce,
                        pattern: EasingPatterns.In
                    );
                };

            LayoutMachine.States["Bubble"].Entered += (ITransition t, IState s) =>
            {
                float? la = null;
                float? ra = null;
                float? ta = null;
                float? ba = null;
                if (t.From == "Hidden")
                {
                    la = -.5f;
                    ra = 0;
                    ta = BubbleLocation.y + .1f;
                    ba = BubbleLocation.y;
                }
                //TODO: update this to have text measurement logic.
                rt.AnimateBounds(MoveTime,
                    fromleftanchor: la,
                    fromrightanchor: ra,
                    fromtopanchor: ta,
                    frombottomanchor: ba,
                    rightanchor: (BubbleLocation.x + 1) / 2,
                    topanchor: BubbleLocation.y + .1f,
                    leftanchor: BubbleLocation.x / 2,
                    bottomanchor: BubbleLocation.y,
                    leftpad: 0,
                    rightpad: 0,
                    toppad: 0,
                    bottompad: 0,
                    curve: EasingCurves.Back,
                    pattern: EasingPatterns.Out);

                TitleRt.AnimateBounds(MoveTime,
                    bottomanchor: 1,
                    curve: EasingCurves.Back,
                    pattern: EasingPatterns.Out);

                BodyRt.AnimateBounds(MoveTime,
                    topanchor: 1,
                    rightpad: 0,
                    bottompad: 0,
                    leftpad: 45,
                    toppad: 0,
                    curve: EasingCurves.Back,
                    pattern: EasingPatterns.Out);
            };
            LayoutMachine.States["Dialogue"].Entered += (ITransition t, IState s) =>
            {
                float? ta = null;
                float? ba = null;
                float? la = null;
                float? ra = null;
                if (t.From == "Hidden")
                {
                    ta = 0;
                    ba = -.3f;
                    la = 0;
                    ra = 1;
                }
                //TODO: update this to have text measurement logic.
                rt.AnimateBounds(MoveTime,
                    fromtopanchor: ta,
                    frombottomanchor: ba,
                    fromleftanchor: la,
                    fromrightanchor: ra,
                    rightanchor: 1,
                    topanchor: .3f,
                    leftanchor: 0,
                    bottomanchor: 0,
                    leftpad: 20,
                    rightpad: 20,
                    toppad: 20,
                    bottompad: 20,
                    curve: EasingCurves.Back,
                    pattern: EasingPatterns.Out);

                TitleRt.AnimateBounds(MoveTime,
                    bottomanchor: .65f,
                    curve: EasingCurves.Back,
                    pattern: EasingPatterns.Out);

                BodyRt.AnimateBounds(MoveTime,
                    topanchor: .65f,
                    rightpad: 10,
                    bottompad: 10,
                    leftpad: 10,
                    toppad: 0,
                    curve: EasingCurves.Back,
                    pattern: EasingPatterns.Out);
            };
            LayoutMachine.States["Fullscreen"].Entered += (ITransition t, IState s) =>
            {
                float? la = null;
                float? ra = null;
                float? ta = null;
                float? ba = null;
                if (t.From == "Hidden")
                {
                    la = 0;
                    ra = 1;
                    ta = 0;
                    ba = -1;
                }
                //TODO: update this to have text measurement logic.
                rt.AnimateBounds(MoveTime,
                    fromleftanchor: la,
                    fromrightanchor: ra,
                    fromtopanchor: ta,
                    frombottomanchor: ba,
                    leftanchor: 0,
                    topanchor: 1,
                    rightanchor: 1,
                    bottomanchor: 0,
                    leftpad: 20,
                    rightpad: 20,
                    toppad: 20,
                    bottompad: 20,
                    curve: EasingCurves.Back,
                    pattern: EasingPatterns.Out);

                TitleRt.AnimateBounds(MoveTime,
                    append: true,
                    cancel: false,
                    bottomanchor: .8f,
                    curve: EasingCurves.Back,
                    pattern: EasingPatterns.Out);

                BodyRt.AnimateBounds(MoveTime,
                    topanchor: .8f,
                    rightpad: 10,
                    bottompad: 10,
                    leftpad: 10,
                    toppad: 0,
                    curve: EasingCurves.Back,
                    pattern: EasingPatterns.Out);
            };

        }


        void OnRectTransformDimensionsChange()
        {
            ss.RefreshLayout();
        }

        public void ToLayout(string state)
        {
            //you need to move each hidden layout to the correct hidden layout before moving to the shown  one.... how do we implement that kindof feautre...
            LayoutMachine.To(state);

        }

        private Vector2 BubbleLocation;
        public void ChangeLocation(Vector2 location)
        {
            //if (LayoutType != LayoutTypes.Bubble)
            //    throw new Exception("Only an in space TextBox has a location, all others exist in screen space");

            BubbleLocation = location;
            ToLayout("Bubble");
        }
        public void Hide()
        {
            LayoutMachine.AdvanceTo("Hidden");
        }


        public void Show()
        {
            if(LayoutMachine.CurrentStateName == "Hidden")
                LayoutMachine.Advance();
        }


        public void ToggleVisible()
        {
            if (LayoutMachine.CurrentStateName.Contains("Hidden"))
                Show();
            else Hide();
        }
    }
}
