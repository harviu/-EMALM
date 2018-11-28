using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void load()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    public void quit()
    {
        Application.Quit();
    }
    public void resume()
    {
        GameObject.Find("Panel").SetActive(false);
    }
}
