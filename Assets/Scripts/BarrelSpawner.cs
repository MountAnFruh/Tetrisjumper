using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelSpawner : MonoBehaviour {

    public GameObject barrel;

	// Use this for initialization
	void Start () {
		
	}
	
	// FixedUpdate
	void FixedUpdate () {
        int value = Random.Range(0, 300);
        if (value == 0)
        {
            GameObject obj = Instantiate(barrel, transform.position, Quaternion.identity);
            Rigidbody2D rigidbody2D = obj.GetComponent<Rigidbody2D>();
            rigidbody2D.AddForce(new Vector2(150, 0));
        }
	}
}
