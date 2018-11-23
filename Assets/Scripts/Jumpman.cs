using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumpman : MonoBehaviour {

    private static float MAX_JUMPCOOLDOWN = 20;
    private static float MAX_JUMPCOOLDOWN_DEAD = 60 * 3;

    public string healer_tag = "Healer_Jumpman";
    public string[] floor_tags = new string[] { "Floor_Jumpman", "Ground_Jumpman" };
    public string[] border_tags = new string[] { "Border_Jumpman" };
    public string[] death_tags = new string[] { "Death" };
    public float speed = 0.05f;
    public float dead_speed = 0.005f;
    public float jumpHeight = 1f;

    private Rigidbody2D body;
    private Animator animator;
    private CapsuleCollider2D bodyCollider;
    private BoxCollider2D feetCollider;
    private float jumpCooldown = 0;

    public bool Dead { get; set; }

    public bool OnGround { get; set; }

    public bool InHealer { get; set; }

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();
        body.isKinematic = false;
        animator = GetComponent<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
    }

    // FixedUpdate
    void FixedUpdate ()
    {
        if (InHealer)
        {
            Heal();
        }

        float moveHorizontal = Input.GetAxis("Jumpman_Horizontal");
        bool jump = Input.GetAxis("Jumpman_Jump") == 1;
        if (moveHorizontal != 0)
        {
            Vector3 scale = transform.localScale;
            if (scale.x < 0)
            {
                scale.x *= moveHorizontal > 0 ? 1 : -1;
            }
            else if (scale.x > 0)
            {
                scale.x *= moveHorizontal > 0 ? -1 : 1;
            }
            transform.localScale = scale;
        }
        if(Dead)
        {
            transform.localPosition += new Vector3(moveHorizontal * dead_speed, 0);
        }
        else
        {
            transform.localPosition += new Vector3(moveHorizontal * speed, 0);
            //body.MovePosition(body.position + new Vector2(moveHorizontal * speed, 0));
        }

        if (jumpCooldown <= 0)
        {
            if (jump && (OnGround || Dead))
            {
                float v0 = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
                Vector2 vel = body.velocity;
                vel.y = v0;
                body.velocity = vel;
                if (Dead)
                {
                    jumpCooldown = MAX_JUMPCOOLDOWN_DEAD;
                }
                else
                {
                    jumpCooldown = MAX_JUMPCOOLDOWN;
                }
            }
        }
        else
        {
            jumpCooldown--;
        }

        animator.SetBool("dead", Dead);
        animator.SetFloat("horizontal_speed", Mathf.Abs(moveHorizontal));
        animator.SetBool("jumping", !OnGround && !Dead);
    }

    void SwitchOnGround()
    {
        OnGround = !OnGround;
    }

    void Heal()
    {
        if (Dead)
        {
            Dead = false;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.AddForce(new Vector2(0, 100f));
            body.rotation = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        for(int i = 0;i < border_tags.Length;i++)
        {
            if (collider.tag == border_tags[i])
            {
                SwitchOnGround();
                return;
            }
        }
        for (int i = 0; i < floor_tags.Length; i++)
        {
            if (collider.tag == floor_tags[i])
            {
                SwitchOnGround();
                return;
            }
        }
        if(collider.tag == healer_tag)
        {
            InHealer = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        for(int j = 0;j < death_tags.Length;j++)
        {
            if (collision.collider.tag == death_tags[j])
            {
                if (!Dead)
                {
                    Dead = true;
                    animator.SetBool("dead", Dead);
                    body.constraints = RigidbodyConstraints2D.None;
                    return;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        for(int i = 0;i < border_tags.Length;i++)
        {
            if(collider.tag == border_tags[i])
            {
                SwitchOnGround();
                return;
            }
        }
        for (int i = 0; i < floor_tags.Length; i++)
        {
            if (collider.tag == floor_tags[i])
            {
                SwitchOnGround();
                return;
            }
        }
        if (collider.tag == healer_tag)
        {
            InHealer = false;
        }
    }

}
