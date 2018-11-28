using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutor2 : MonoBehaviour {

    private GameObject tutor;
    private Text text;
    // Use this for initialization
    void Start()
    {
        tutor = GameObject.Find("tutorText");
        text = tutor.GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            text.text = "Part of the map is revealed\n Hold Q and click on the minimap to teleport";
        }
    }

}
