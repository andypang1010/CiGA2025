using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // Make the sprite face the camera
        transform.forward = cam.transform.forward;
    }
}