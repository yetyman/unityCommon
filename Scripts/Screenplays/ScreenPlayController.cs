using Assets.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScreenPlayController : MonoBehaviour {

    private Dictionary<string, Conversation> TriggeredConversations = new Dictionary<string, Conversation>();
    private ScreenPlay sp;
    //dont have a current conversation member. would be fun to have two coroutines with two characters arguing. would be pretty cool and robust too..
    

	// Use this for initialization
	void Start () {
        GameSceneContext.ScreenPlay = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeScript(TextAsset rawScript)
    {
        //first read in the json model.
        var newScreenPlay = ScreenPlay.FromJSON(rawScript.text);
        if (newScreenPlay != null)
        {
            sp = newScreenPlay;
            //now put all the events in a dictionary
            TriggeredConversations = new Dictionary<string, Conversation>();
            foreach (var convo in sp.Conversations)
                if (!string.IsNullOrEmpty(convo.Trigger))
                    TriggeredConversations.Add(convo.Trigger, convo);
            //if there is a starting conversation, start the starting conversation.
            var first = sp.Conversations.FirstOrDefault(x => x.Trigger == "Default");
            if (first != null)
                BeginConvo(first.TextboxInstance, first);
        }
    }
    public void BeginConvo(string textbox, string conversationName)
    {
        if (TriggeredConversations.ContainsKey(conversationName))
            BeginConvo(textbox, TriggeredConversations[conversationName]);
    }
    public void BeginConvo(string textBox, string[] conversation, Vector3? speakerLoc = null, string speakerName = null)
    {
        BeginConvo(textBox, new Conversation() {
            Name = "SideConversation",
            TextboxInstance = (speakerLoc == null) ? "Main" : "Small",
            Dialogues = conversation.Select<string, Dialogue>(v => new Dialogue() { Text = v, SName = speakerName }).ToList()
        });
    }
    private string TxtBoxCurrentConvo(string textbox)
    {
        return "Current-" + textbox;
    }
    public void BeginConvo(string textBox, Conversation convo)
    {
        convo.CurrentPosition = -1;

        StartCoroutine(Converse(convo));
    }

    public void ContinueConvo(string textBox)
    {
        if (TriggeredConversations.ContainsKey(TxtBoxCurrentConvo(textBox)))
        {
            var convo = TriggeredConversations[TxtBoxCurrentConvo(textBox)];

            TriggeredConversations.Remove(TxtBoxCurrentConvo(textBox));
            StartCoroutine(Converse(convo));
        }
    }

    IEnumerator Converse(Conversation convo)
    {
        if (convo.CurrentPosition == null) convo.CurrentPosition = -1;

        for (; convo.CurrentPosition < convo.Dialogues.Count(); )
        {
            convo.CurrentPosition++;

            Dialogue current = null;
            if (convo.Dialogues.Count > convo.CurrentPosition)
               current = convo.Dialogues[convo.CurrentPosition.Value];
            //commands ahappen before you set text because you may want to create a new textbox for a given piece of text. and that would be a command.

            if (current == null)
            {
                EndConvo(convo);
                break;
            }
            else
            {
                yield return new WaitUntil(()=> GameSceneContext.TextBoxes.ContainsKey(convo.TextboxInstance));//this is just in case an image for a new text box is taking fucking forever to load or something. bad code. don't do it this way.

                HandleCommands(current);
                SetText(current);
                if (convo.CurrentPosition < convo.Dialogues.Count() - 1)
                {
                    float waitTime = -1;
                    var next = convo.Dialogues[convo.CurrentPosition.Value + 1];

                    waitTime = SetupNext(next);
                    if (waitTime >= 0)
                        yield return new WaitForSeconds(waitTime);
                    if (next.StartRequirement == "ContinueButton" || next.StartRequirement == "")
                        yield break;
                }
                else {
                    SetupNext(new Dialogue() { Parent = convo });
                    yield break;
                }
            }
        }
    }

    private void SetText(Dialogue current)
    {
        //TODO: Break this out into a function that retries for a few seconds until it works or until something else requests to use the text box, consider having the set Dialogue method return  a successful or not boo
        GameSceneContext.TextBoxes[current.Parent.TextboxInstance].SetDialogue(current.Text, current.SName, current.DisplayName, current.DisplayBox);
    }

    private void HandleCommands(Dialogue current)
    {
        foreach (Command cmd in current.Commands)
        {
            //TODO: do stuff.
            //for instance "Move Character:" or "CloseTextBox"
        }
    }
    private float SetupNext(Dialogue next)
    {
        if (next.StartRequirement == "ContinueButton" || string.IsNullOrEmpty(next.StartRequirement))
        {
            GameSceneContext.TextBoxes[next.Parent.TextboxInstance].SetClickable(true);
            if (TriggeredConversations.ContainsKey(TxtBoxCurrentConvo(next.Parent.TextboxInstance)))
                TriggeredConversations.Remove(TxtBoxCurrentConvo(next.Parent.TextboxInstance));
            TriggeredConversations.Add(TxtBoxCurrentConvo(next.Parent.TextboxInstance), next.Parent);

            //wait for something to tell the screenplay controller to advance text. this will probably but a button in the text box controller that is also receiving keyboard events, but that's not something you determine in here
            //add to triggers a conversation with the name "Current", this is fine because normally a secondary text box would be controlled by other events than the user's regular input.
        }
        else if (next.StartRequirement.StartsWith("Time:"))
        {

            float time = 0.0f;
            if (float.TryParse(next.StartRequirement.Substring("Time:".Length).Trim(), out time))
                return time;
            else Debug.LogWarning("something is wrong with the format of your Dialogue. Time: ???s " + next.StartRequirement);
        }
        return -1;
    }
    private void EndConvo(Conversation convo)
    {
        convo.CurrentPosition = 0;
        var last = convo.Dialogues.Last();
        //GameSceneContext.TextBoxes[convo.TextboxInstance].SetDialogue("", last.SName, last.DisplayName, last.DisplayBox);
        GameSceneContext.TextBoxes[convo.TextboxInstance].Hide();
    }
}
