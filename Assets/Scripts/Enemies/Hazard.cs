using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public string chars;
    public float changeTime = 0.01f;
    public TextMeshProUGUI textBox;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ChangeChar", 0, changeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChangeChar()
    {
        char[] charArray = chars.ToCharArray();
        char theChosenOne = charArray[Random.Range(0, charArray.Length)];
        textBox.text = theChosenOne.ToString();
    }
}
