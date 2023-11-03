using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallOfDeath : MonoBehaviour
{
    public bool moving = true;
    public float moveSpeed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
            transform.Translate(new Vector3(moveSpeed, 0, 0) * Time.deltaTime);
    }
}
