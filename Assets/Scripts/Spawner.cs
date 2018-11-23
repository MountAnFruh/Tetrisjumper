using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject[] groups;

    public void spawnNext()
    {
        int i = Random.Range(0, groups.Length);

        Group group = Instantiate(groups[i],
            transform.position,
            Quaternion.identity).GetComponent<Group>();
        group.Spawner = this;
    }

	// Use this for initialization
	void Start () {
        spawnNext();
	}
}
