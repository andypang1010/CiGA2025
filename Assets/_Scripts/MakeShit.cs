using UnityEngine;

public class MakeShit : MonoBehaviour
{
    public GameObject shit;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject someShit = Instantiate(shit, new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 1), Quaternion.identity);
            //shit.GetComponent<Rigidbody2D>().AddForce()
            
        }
    }
}
