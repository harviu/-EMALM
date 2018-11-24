using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTexture : MonoBehaviour {
	public float shuffleInterval = 5f;
    public float pivit = 0.4f;

    float lastShuffle;
    float offsetX;
    float offsetY;
    float tile, scollY,z;
    Renderer r;


    void Start()
    {
        r = GetComponent<Renderer>();
        lastShuffle = Time.time;

        lastShuffle = Time.time;

        offsetX = Random.Range(0f, 1f);
        offsetY = Random.Range(0f, 1f);
        scollY = Random.Range(0.05f, 0.24f);

        tile = Random.Range(0.2f, 0.5f);
        r.material.mainTextureScale = new Vector2(tile, tile);
        //Debug.Log(scollY);
    }
	// Update is called once per frame
	void Update () {
        if (Time.time - lastShuffle > 5)
        {
            lastShuffle = Time.time;
            
            scollY = Random.Range(pivit / 2 - 0.05f, pivit / 2 + 0.05f);
        }

		offsetY += Time.deltaTime *scollY;
		r.material.mainTextureOffset = new Vector2 (offsetX, offsetY);
        

    }
}
