using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public bool isRetractable = false;

    private float MAX_RETRACTCOOLDOWN = 60*10; // 60 Frames UP - 60 Frames DOWN = 2 + 3 between

    private Jumpman currJumpman;

    private float retractCooldown;
    
    private Transform upperChild;
    private float[] toMoveArr;

    // Use this for initialization
    void Start()
    {
        toMoveArr = new float[this.transform.childCount];
        upperChild = this.transform.GetChild(0);
        for (int i = 1; i < this.transform.childCount; i++)
        {
            Transform transformChild = this.transform.GetChild(i);
            if (transformChild.position.y > upperChild.position.y)
            {
                upperChild = transformChild;
            }
        }
        for (int i = 0;i < this.transform.childCount; i++)
        {
            Transform transformChild = this.transform.GetChild(i);
            float diffY = upperChild.position.y - transformChild.position.y;
            float toMove = diffY / 30;
            toMoveArr[i] = toMove;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (retractCooldown > 0)
        {
            if(retractCooldown <= 30) // Animation upwards
            {
                for (int i = 0;i < this.transform.childCount;i++)
                {
                    Transform transformChild = this.transform.GetChild(i);
                    transformChild.position -= new Vector3(0,toMoveArr[i],0);
                }
            }
            else if(retractCooldown >= MAX_RETRACTCOOLDOWN - 30 + 1) // Animation downwards
            {
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    Transform transformChild = this.transform.GetChild(i);
                    transformChild.position += new Vector3(0, toMoveArr[i], 0);
                }
            }
            retractCooldown--;
        }
        else
        {
            GetComponent<Collider2D>().enabled = true;
            int value = Random.Range(0, 300);
            //DEBUG:
            //int value = 10;
            //
            if(value == 0 && isRetractable && this.transform.childCount > 0)
            {
                MAX_RETRACTCOOLDOWN = 60 * 10 - Random.Range(0, 60 * 6);
                retractCooldown = MAX_RETRACTCOOLDOWN;
                GetComponent<Collider2D>().enabled = false;
                return;
            }
            bool jump = Input.GetAxis("Jumpman_Jump") == 1;
            bool crouch = Input.GetAxis("Jumpman_Crouch") == 1;
            if (currJumpman != null && !currJumpman.Dead)
            {
                Debug.Log("IFSS " + jump + " " + crouch + " " + currJumpman.OnLadder);
                if ((jump || crouch) && !currJumpman.OnLadder && currJumpman.OnGround)
                {
                    Debug.Log("SET PARAMETERS");
                    currJumpman.GetComponent<Rigidbody2D>().isKinematic = true;
                    currJumpman.OnLadder = true;
                    currJumpman.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                }

                if (jump)
                {
                    Debug.Log("JUMPPP");
                    currJumpman.GetComponent<Rigidbody2D>().MovePosition(currJumpman.GetComponent<Rigidbody2D>().position + new Vector2(0, 0.05f));
                }
                else if (crouch)
                {
                    currJumpman.GetComponent<Rigidbody2D>().MovePosition(currJumpman.GetComponent<Rigidbody2D>().position - new Vector2(0, 0.05f));
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Jumpman jumpman = collider.GetComponent<Jumpman>();
        if (jumpman != null && !jumpman.Dead)
        {
            currJumpman = jumpman;
            currJumpman.JumpDisabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        Jumpman jumpman = collider.GetComponent<Jumpman>();
        if (jumpman != null && currJumpman != null && !jumpman.Dead)
        {
            currJumpman.GetComponent<Rigidbody2D>().isKinematic = false;
            currJumpman.JumpDisabled = false;
            currJumpman.OnLadder = false;
            currJumpman = null;
        }
    }
}
