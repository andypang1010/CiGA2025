using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float groundDistance;

    public LayerMask groundLayer;
    public SpriteRenderer sr;
    Rigidbody rb;

    Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        moveDirection = new Vector3(x, 0, y).normalized;

        // if (x != 0 && x < 0)
        // {
        //     sr.flipX = true;
        // }

        // else if (x != 0 && x > 0)
        // {
        //     sr.flipX = false;
        // }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }
}