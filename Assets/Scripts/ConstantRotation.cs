using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    public Transform trans;
    public Vector3 rotationPerSecond;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        trans.Rotate(rotationPerSecond * Time.deltaTime);
    }
}
