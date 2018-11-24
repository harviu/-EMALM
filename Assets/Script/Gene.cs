using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Gene : MonoBehaviour {

    private GameObject player;
    public int geneIndex;
	// Use this for initialization
	void Start () {
        player = GameObject.Find("FPSController");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject.Find("blockPlane" + geneIndex).SetActive(false);
            Destroy(GameObject.Find("blockPlane" + geneIndex));
            gameObject.GetComponents<AudioSource>()[1].Stop();
            gameObject.SetActive(false);
            Destroy(gameObject);
            player.GetComponent<FirstPersonController>().m_GeneArray = GameObject.FindGameObjectsWithTag("Target");
            GameObject[] array = player.GetComponent<FirstPersonController>().m_GeneArray;
            Debug.Log(array.Length);
            if (array.Length == 0)
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
    }
}
