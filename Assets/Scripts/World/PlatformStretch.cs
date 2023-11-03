using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformStretch : MonoBehaviour
{
    public Transform interior;
    public SpriteRenderer equals;
    
    Vector3 stretchDimensions = Vector3.one;
    
    // Start is called before the first frame update
    void Start()
    {
        stretchDimensions = transform.localScale;
        
        interior.localScale = new Vector3(stretchDimensions.x * (7/6), stretchDimensions.y, stretchDimensions.z);
        equals.size = stretchDimensions;

        transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    float InteriorX()
    {
        float x = stretchDimensions.x;
        return (2 * x) / (1.71f * x + 0.15f);
    }
}
