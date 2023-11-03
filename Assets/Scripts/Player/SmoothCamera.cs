using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    public float dampTime = 0.15f;
    public Transform target;
    public Vector3 offset;

    private Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!target) return;

        Vector3 smoothMoves = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, dampTime);
        transform.position = new Vector3(smoothMoves.x, transform.position.y, transform.position.z);
    }
}
