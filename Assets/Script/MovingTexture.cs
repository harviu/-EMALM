using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTexture : MonoBehaviour {
	public float shuffleInterval = 5f;
    public float speedP = 0.3f;
    public float tileXP = 0.4f;

    private float rat;
    float lastShuffle;
    float offsetX;
    float offsetY;
    float tileX,tileY, scollY,z;
    Renderer r;


    void Start()
    {
        //get the dimension of the plane
        rat = transform.localScale.x/ transform.localScale.z;


        r = GetComponent<Renderer>();
        lastShuffle = Time.time;

        //random starting point
        offsetX = Random.Range(0f, 1f);
        offsetY = Random.Range(0f, 1f);

        //random speed
        //scollY = Random.Range(0f, speedP);
        scollY = speedP;

        //random tile
        //tileX = Random.Range(0.2f, tileXP);
        tileX = tileXP;
        tileY = tileX/rat*16f/9f;

        r.material.mainTextureScale = new Vector2(tileX, tileY);
        //Debug.Log(scollY);
    }
	// Update is called once per frame
	void Update () {
        //shuffle speed
        /*
        if (Time.time - lastShuffle > shuffleInterval)
        {
            lastShuffle = Time.time;
            
            scollY = Random.Range(0f, speedP);
        }*/

		offsetY += Time.deltaTime *scollY;
		r.material.mainTextureOffset = new Vector2 (offsetX, offsetY);
        

    }
}
