using UnityEngine;

public class CallShitFunc : MonoBehaviour
{
    [SerializeField] Cow cow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void callPoop()
    {
        cow.Poop();
    }
}
