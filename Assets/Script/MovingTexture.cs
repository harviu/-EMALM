using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTexture : MonoBehaviour {
	public float shuffleInterval = 5f;
    public float speedP = 0.3f;
    public float tileXP = 0.4f;
    public float rat = 1;

    float lastShuffle;
    float offsetX;
    float offsetY;
    float tileX,tileY, scollY,z;
    Renderer r;


    void Start()
    {
        r = GetComponent<Renderer>();
        lastShuffle = Time.time;

        lastShuffle = Time.time;

        offsetX = Random.Range(0f, 1f);
        offsetY = Random.Range(0f, 1f);
        scollY = Random.Range(0f, speedP);

        tileX = Random.Range(0.2f, tileXP);
        tileY = tileX/rat;

        r.material.mainTextureScale = new Vector2(tileX, tileY);
        //Debug.Log(scollY);
    }
	// Update is called once per frame
	void Update () {
        if (Time.time - lastShuffle > 5)
        {
            lastShuffle = Time.time;
            
            scollY = Random.Range(0f, speedP);
        }

		offsetY += Time.deltaTime *scollY;
		r.material.mainTextureOffset = new Vector2 (offsetX, offsetY);
        

    }
}
