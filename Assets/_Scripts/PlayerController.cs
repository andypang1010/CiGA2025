using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public SpriteRenderer sr;
    Rigidbody rb;
    bool interactPressed;
    Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        interactPressed = Input.GetKeyDown(KeyCode.E);
        moveDirection = new Vector3(x, 0, y).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;


        Vector3 castPos = transform.position;
        GameObject[] interactables = Physics.OverlapSphere(transform.position,
            1f, LayerMask.GetMask("Interactable")).Select(collider => collider.gameObject).ToArray();

        if (interactables.Length == 0)
        {
            return;
        }

        float closestDistance = Mathf.Infinity;
        GameObject closestInteractable = null;

        foreach (GameObject interactable in interactables)
        {
            float currentDistance = Vector3.Distance(transform.position, interactable.transform.position);
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                closestInteractable = interactable;
            }
        }

        // get the interaction type
        InteractionType interactionType = InteractionType.Pickup;
        // MORE LOGIC： TODO

        // try casting interactable to an Interactable
        if (closestInteractable != null)
        {
            try
            {
                Interactable interactable = closestInteractable.GetComponent<Interactable>();

                if (interactable != null && interactable.CanInteract())
                {
                    if (interactPressed)
                    {
                        interactable.React(interactionType);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error interacting with {closestInteractable.name}: {e.Message}, It is not castable to Interactable!");
            }
        }
    }
}