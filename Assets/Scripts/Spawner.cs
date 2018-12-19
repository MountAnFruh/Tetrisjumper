using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject[] groups;
    public int[] history = new int[4] { 4,4,6,6 };
    public int[] nextQueue = new int[4];
    public GameObject[] nextQueueObjects = new GameObject[4];
    public BarrelSpawner barrel;

    public int randomizer()
    {
        bool same;
        int i;
        int tries = 0;
        do
        {
            same = false;
            i = Random.Range(0, groups.Length);
            foreach (int j in history)
            {
                if (i == j)
                    same = true;
            }
            tries++;
            //Debug.Log("Try: " + tries + " Piece: " + i + " Same? " + same);
        } while (tries < 6 && same == true);

        history[3] = history[2];
        history[2] = history[1];
        history[1] = history[0];
        history[0] = i;
        //Debug.Log("Piece: " + i + " Tries: " + tries);
        return i;
    }

    public void initNextQueue()
    {
        nextQueue[0] = randomizer();
        nextQueue[1] = randomizer();
        nextQueue[2] = randomizer();
        nextQueue[3] = randomizer();
    }

    public void addToNextQueue()
    {
        nextQueue[0] = nextQueue[1];
        nextQueue[1] = nextQueue[2];
        nextQueue[2] = nextQueue[3];
        nextQueue[3] = randomizer();
    }

    public void showNextQueue()
    {
        foreach (GameObject nextQueueObject in nextQueueObjects)
        {
            if (nextQueueObject != null)
                nextQueueObject.active = false;
        }
        nextQueueObjects[0] = (GameObject) Instantiate(groups[nextQueue[0]],
            transform.position + new Vector3(7, 0, -3),
            Quaternion.identity);
        nextQueueObjects[1] = (GameObject)Instantiate(groups[nextQueue[1]],
            transform.position + new Vector3(7, -4, -3),
            Quaternion.identity);
        nextQueueObjects[2] = (GameObject)Instantiate(groups[nextQueue[2]],
            transform.position + new Vector3(7, -8, -3),
            Quaternion.identity);
        nextQueueObjects[3] = (GameObject)Instantiate(groups[nextQueue[3]],
            transform.position + new Vector3(7, -12, -3),
            Quaternion.identity);
        nextQueueObjects[0].GetComponent<Group>().enabled = false;
        nextQueueObjects[1].GetComponent<Group>().enabled = false;
        nextQueueObjects[2].GetComponent<Group>().enabled = false;
        nextQueueObjects[3].GetComponent<Group>().enabled = false;
    }

    public void hold()
    {

    }

    public void spawnNext()
    {
        int i = nextQueue[0];
        addToNextQueue();
        showNextQueue();
        //Debug.Log(nextQueue[0] + " " + nextQueue[1] + " " + nextQueue[2] + " " + nextQueue[3]);

        Group group = Instantiate(groups[i],
            transform.position,
            Quaternion.identity).GetComponent<Group>();
        group.Spawner = this;
    }

	// Use this for initialization
	void Start () {
        initNextQueue();
        spawnNext();
	}
}
