using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private Jumpman currJumpman;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool jump = Input.GetAxis("Jumpman_Jump") == 1;
        if (currJumpman != null)
        {
            if (jump)
            {
                //jumpman.transform.position -= new Vector3(0, -0.5f, 0);
                currJumpman.GetComponent<Rigidbody2D>().isKinematic = true;
                currJumpman.GetComponent<Rigidbody2D>().MovePosition(currJumpman.GetComponent<Rigidbody2D>().position + new Vector2(0, 0.5f));
            }
            else
            {
                currJumpman.GetComponent<Rigidbody2D>().isKinematic = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Jumpman jumpman = collider.GetComponent<Jumpman>();
        if (jumpman != null)
        {
            currJumpman = jumpman;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        Jumpman jumpman = collider.GetComponent<Jumpman>();
        if (jumpman != null)
        {
            currJumpman = null;
        }
    }
}
