using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
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
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed + Vector3.down * rb.linearVelocity.y;
    }
}