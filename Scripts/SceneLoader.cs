using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    public string DefaultScene = "Scene1TestTest";
    public bool debug = false;
	// Use this for initialization
	void Start () {
        if(!debug)
            SceneManager.LoadScene(DefaultScene, LoadSceneMode.Additive);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
