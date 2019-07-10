using Assets.Scripts.Maps;
using Assets.Scripts.TextBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class TextBoxControllerDictionary : SerializableDictionary<string, TextBoxController> { };
[Serializable] public class SpriteDictionary : SerializableDictionary<string, Sprite> { };
[Serializable] public class SpriteReverseDictionary : SerializableDictionary<Sprite, string> { };
[Serializable] public class ObjectDictionary : SerializableDictionary<string, object> { };
[Serializable] public class UITextDictionary : SerializableDictionary<string, UITextElement> { };
[Serializable] public class IndexToIndexDictionary : SerializableDictionary<int, int> { };

[Serializable]
public static class GameSceneContext {
    public static ObjectDictionary OtherItems = new ObjectDictionary();

    public static IMap Map;
    public static ScreenPlayController ScreenPlay = null;

    public static TextBoxControllerDictionary TextBoxes = new TextBoxControllerDictionary();

    public static SpriteDictionary BGs = new SpriteDictionary();
    public static SpriteReverseDictionary BGsToNames = new SpriteReverseDictionary();
    internal static UITextDictionary UIElements = new UITextDictionary();
}
