using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

public class MonsterFollow : MonoBehaviour {

    private NavMeshAgent agent;
    private GameObject player;
	// Use this for initialization
	void Start () {
        player = GameObject.Find("FPSController");
        agent = gameObject.GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        agent.enabled = true;
        agent.SetDestination(player.transform.position);
        agent.speed = 0f;
        if (/*Vector3.Distance(gameObject.transform.position, player.transform.position) < 12 
            &&  */!agent.pathPending)
        {
            agent.speed = 2.5f;
        }
        else
        {
            agent.speed = 0f;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        col.transform.position = new Vector3(0f, 1f, 0f);
        player.GetComponent<FirstPersonController>().m_DataIntegrity = 10;
        Debug.Log("10/10");
    }

}
