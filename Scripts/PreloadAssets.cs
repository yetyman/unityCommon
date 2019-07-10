using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreloadAssets : MonoBehaviour {

	// Use this for initialization
	void Start () {
        foreach (var sp in BGs)
        {
            GameSceneContext.BGs.Add(sp.Key, sp.Value);
            GameSceneContext.BGsToNames.Add(sp.Value, sp.Key);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static SpriteDictionary BGs = new SpriteDictionary();

}
