using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public SpriteRenderer sr;
    public Animator animator;

    public float speed = 3;
    public Vector3 moveDirection;
    public Vector3 faceDirection;
    Rigidbody rb;
    public AudioSource footstep;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        footstep.Play(0);
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        moveDirection = new Vector3(x, 0, y).normalized;

        // update animator parameters
        animator.SetFloat("Speed", moveDirection.magnitude);

        // if x and y are not zero, update facedirection; otherwise keep the previous
        if (Mathf.Abs(x) > 0.01f || Mathf.Abs(y) > 0.01f)
        {
            faceDirection = new Vector3(x, 0, y).normalized;
            sr.flipX = faceDirection.x < 0;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)|| Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            footstep.UnPause();
            Debug.Log("play walk sound");
        }
        else
        {
            footstep.Pause();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }
}