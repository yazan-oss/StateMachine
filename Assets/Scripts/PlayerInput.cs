using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Animator animator;
    public float groundCheckDistance = 0.1f;
    public float wallRaycastDistance = 0.6f;
    public ContactFilter2D groundCheckFilter;

    private Rigidbody2D rb;
    private Collider2D collider2d;
    private List<RaycastHit2D> groundHits = new List<RaycastHit2D>();
    private List<RaycastHit2D> wallHits = new List<RaycastHit2D>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
    }

    // Update is called once per frame

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");

        animator.SetFloat("moveX", moveX);

        bool isMoving = !Mathf.Approximately(moveX, 0f);

        animator.SetBool("isMoving", isMoving);

        // Check & Trigger for On Ground
        bool lastOnGround = animator.GetBool("isOnGround");
        bool newOnGround = CheckIfOnGround();
        animator.SetBool("isOnGround", newOnGround);

        if (lastOnGround == false && newOnGround == true)
        {
            animator.SetTrigger("landedOnGround");
        }

        // Check & Trigger for On Wall
        bool onWall = CheckIfOnWall();
        animator.SetBool("isOnWall", onWall);

        // Jump
        bool isJumpKeyPressed = Input.GetButtonDown("Jump");

        if (isJumpKeyPressed)
        {
            animator.SetTrigger("jump");
        }
        else
        {
            animator.ResetTrigger("jump");
        }
    }

    void FixedUpdate()
    {
        float forceX = animator.GetFloat("forceX");

        if (forceX != 0) rb.AddForce(new Vector2(forceX, 0) * Time.deltaTime);

        float impulseY = animator.GetFloat("impulseY");
        float impulseX = animator.GetFloat("impulseX");

        if (impulseY != 0 || impulseX != 0)
        {
            float xDirectionSign = Mathf.Sign(transform.localScale.x); //wall jump while write direction
            Vector2 impulseVector = new Vector2(xDirectionSign * impulseX, impulseY);

            rb.AddForce(impulseVector, ForceMode2D.Impulse);
            animator.SetFloat("impulseY", 0);
            animator.SetFloat("impulseX", 0);
        }

        animator.SetFloat("velocityY", rb.velocity.y);

        bool isStopVelocity = animator.GetBool("stopVelocity");

        if (isStopVelocity)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("stopVelocity", false);
        }
    }

    // Uses a collider cast on the attached collider2d to check if there are any
    // gameobjects collided with on the layers set in groundCheckFilter

    bool CheckIfOnGround()
    {
        collider2d.Cast(Vector2.down, groundCheckFilter, groundHits, groundCheckDistance);

        if (groundHits.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Checks for wall collisions ahead of the player in the direction that it is
    // currently facing.

    bool CheckIfOnWall()
    {
        Vector2 localScale = transform.localScale;

        collider2d.Raycast(Mathf.Sign(localScale.x) * Vector2.right, groundCheckFilter, wallHits, wallRaycastDistance);

        if (wallHits.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
