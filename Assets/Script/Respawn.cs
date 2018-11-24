using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.Characters.FirstPerson;

public class Respawn : MonoBehaviour {

    private GameObject player;
    // Use this for initialization
    void Start () {
        player = GameObject.Find("FPSController");

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DebugPoint()
    {
        PointerEventData ped= new PointerEventData(EventSystem.current);
        ped.position = Input.mousePosition;
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), ped.position, ped.pressEventCamera, out localCursor))
            return;
        float width = gameObject.GetComponent<RectTransform>().rect.width;
        float locX = localCursor.x / width * 240f;
        float locZ = localCursor.y / width * 240f;
        Vector3 checkPos = new Vector3(locX, 60f, locZ);
        if (!Physics.CheckSphere(checkPos, 0.5f, ~(1 << 8)))
        {
            player.transform.position = new Vector3(locX, 1, locZ);
            player.GetComponent<FirstPersonController>().m_DataIntegrity = 10;
            Debug.Log("10/10");
        }

    }
}
