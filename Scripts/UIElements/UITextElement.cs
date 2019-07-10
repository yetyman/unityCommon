using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UITextElement : MonoBehaviour
{
    public TextMeshPro Text;
    // Start is called before the first frame update
    void Start()
    {
        if(Text == null)
            Text = GetComponent<TextMeshPro>();
        if (name == null)
            throw new System.Exception("UIElements must have a name they can be found with");
        GameSceneContext.UIElements.Add(name, this);
    }

    public void UpdateText(string text)
    {
        Text.SetText(text);
    }
    public void UpdateText(Dictionary<string, string> text)
    {
        UpdateText(text.Select((kv) => $"{kv.Key}: {kv.Value}"));
    }
    public void UpdateText(IEnumerable<string> text)
    {
        UpdateText(string.Join("\n", text));
    }

}
