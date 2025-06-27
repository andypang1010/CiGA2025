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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }
}