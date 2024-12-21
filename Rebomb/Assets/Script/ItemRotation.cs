using UnityEngine;

public class ItemRotation : MonoBehaviour
{
    public bool Rotating = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Rotating)
        {
            transform.Rotate(0, 50 * Time.deltaTime, 0, Space.Self);
        }
    }
}
