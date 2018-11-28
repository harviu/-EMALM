using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutor : MonoBehaviour {
    private GameObject tutor;
	// Use this for initialization
	void Start () {
        tutor = GameObject.Find("Tutor");
        tutor.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            tutor.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            tutor.SetActive(false);
        }
    }

}
