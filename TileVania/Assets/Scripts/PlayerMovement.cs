using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float jumpSpeed = 11f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] PhysicsMaterial2D deathPhysics;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;

    Vector2 moveInput;
    Rigidbody2D rb;
    CapsuleCollider2D bodyCollider;
    BoxCollider2D feetCollider;
    Animator animator;

    float gravity;

    bool IsAlive = true;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        gravity = rb.gravityScale;
    }

    void Update()
    {
        if (!IsAlive) return;

        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnMove(InputValue value)
    {
        if (!IsAlive) return;

        moveInput = value.Get<Vector2>();

    }


    void OnJump(InputValue value)
    {
        if (!IsAlive) return;

        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;
        
        if (value.isPressed) 
        {
            rb.velocity += new Vector2(0f, jumpSpeed);

        }
    }

    void OnFire(InputValue value)
    {
        if (!IsAlive) return;

        if (value.isPressed)
        {
            Instantiate(bullet, gun.position, transform.rotation);
        }
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;


        bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon; //0
        animator.SetBool("IsRunning", playerHasHorizontalSpeed);

    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon; //0

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
        }
    }

    void ClimbLadder()
    {
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            rb.gravityScale = gravity;
            animator.SetBool("IsClimbing", false);
            return;
        }

        Vector2 climbVelocity = new Vector2(rb.velocity.x, moveInput.y * climbSpeed);
        rb.velocity = climbVelocity;
        rb.gravityScale = 0;

        bool playerHasVerticalSpeed = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;
        animator.SetBool("IsClimbing", playerHasVerticalSpeed);

    }

    void Die()
    {
        if (bodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            IsAlive = false;
            animator.SetTrigger("Dying"); 
            rb.velocity = new Vector2(rb.velocity.x, 15f);
            bodyCollider.sharedMaterial = deathPhysics;
            StartCoroutine(ProcessDeath());
        }
    }

    IEnumerator ProcessDeath()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        FindObjectOfType<GameSession>().ProcessPlayerDeath();
    }
}
