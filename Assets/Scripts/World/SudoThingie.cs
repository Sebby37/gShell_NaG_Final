using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudoThingie : MonoBehaviour
{
    public Sprite[] sudoChars;
    public static int sudoCharIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAllSudos()
    {
        if (sudoCharIndex < 3)
            sudoCharIndex++;

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("SUDO"))
            g.GetComponent<SpriteRenderer>().sprite = sudoChars[sudoCharIndex];
    }
}
