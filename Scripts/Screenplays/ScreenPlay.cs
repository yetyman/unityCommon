using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Models
{
    [Serializable]
    public class ScreenPlay
    {
        public List<Conversation> Conversations = new List<Conversation>();
        public static ScreenPlay FromJSON(string text)
        {
            var sp = JsonUtility.FromJson<ScreenPlay>(text);
            foreach (Conversation convo in sp.Conversations)
            {
                convo.Dialogues.ForEach(x => x.Commands = x.Commands ?? new List<Command>());

                //set default text box
                if (string.IsNullOrEmpty(convo.TextboxInstance))
                    convo.TextboxInstance = "Main";

                //i dont think this part should matter for execution, but it'll probably make debuggin a looot easier down the road
                var lastdlg = convo.Dialogues.First();
                foreach (Dialogue d in convo.Dialogues)
                {
                    if (string.IsNullOrEmpty(d.SName))
                        d.SName = lastdlg.SName;
                    d.Parent = convo;
                    if (string.IsNullOrEmpty(d.StartRequirement))
                        d.StartRequirement = "ContinueButton";

                    lastdlg = d;
                }
            }
            return sp;
        }
    }
    [Serializable]
    public class Conversation
    {
        public string Name = "";
        public List<Dialogue> Dialogues = new List<Dialogue>();
        public string Trigger = "";
        public string TextboxInstance = null;
        public int? CurrentPosition = null;
    }
    [Serializable]
    public class Dialogue
    {
        [NonSerialized]
        public Conversation Parent = null;
        public string SName = "";//speaker name
        public string DisplayName = null;
        public string DisplayBox = null;
        public string Text = "";
        public string StartRequirement = null;
        public List<Command> Commands = new List<Command>();

        public override string ToString()
        {
            return SName + (!string.IsNullOrEmpty(DisplayName) && DisplayName != SName ? " (" + DisplayName + ")" : "") + " : " + (string.IsNullOrEmpty(StartRequirement) ? "" : "(when " + StartRequirement + ")") + Text + " ( " + string.Join(",", Commands.Select(x => x.Action).ToArray()) + " )";
        }

        public bool IsFirst()
        {
            return Parent.Dialogues.First() == this;
        }
    }
    [Serializable]
    public class Command
    {
        public string Action = "";
        public string Target = "";
    }

}
