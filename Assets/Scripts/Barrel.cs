using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour {

    public string ignore_tag = "Ground_Jumpman";
    private new Transform transform;
    private new Rigidbody2D rigidbody2D;
    
	void Start () {
        transform = GetComponent<Transform>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(new Vector2(150, 0));

        GameObject[] gameObjs = GameObject.FindGameObjectsWithTag(ignore_tag);
        for (int i = 0; i < gameObjs.Length; i++)
        {
            GameObject gameObj = gameObjs[i];
            Physics2D.IgnoreCollision(rigidbody2D.GetComponent<Collider2D>(), gameObj.GetComponent<Collider2D>());
        }
    }
	
	void FixedUpdate () {
        
	}
}
