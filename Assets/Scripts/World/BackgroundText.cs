using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BackgroundText : MonoBehaviour
{
    public float timeBetweenNextChunk = 0.05f;
    public int chunkSize = 5;
    public int maxInitialRandText = 2500;
    
    static string textLines = null;
    TextMeshProUGUI textBox;
    int curTextIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Get da bocks
        textBox = GetComponentInChildren<TextMeshProUGUI>();

        // Load it
        LoadText();

        // Set the index
        curTextIndex = Random.Range(0, textLines.Length - 60);

        // Begin the chunk calling
        InvokeRepeating("NextChunk", 0.0f, timeBetweenNextChunk);

        // Set some initial text for fun y'know?
        int randInitialText = Random.Range(0, maxInitialRandText);
        for (int i = 0; i < maxInitialRandText; i++)
        {
            textBox.text += textLines[curTextIndex % textLines.Length];
            curTextIndex++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void NextChunk()
    {
        // Clearing the textbox if there are too many lines
        if (textBox.text.Split('\n').Length >= 86)
            textBox.text = "";
        
        // Creating and adding the chunk
        string chunk = "";
        for (int i = 0; i < chunkSize; i++)
        {
            chunk += textLines[curTextIndex % textLines.Length];
            curTextIndex++;
        }
        textBox.text += chunk;
    }

    void LoadText()
    {
        // No bother
        if (textLines != null)
            return;

        // Load text
        textLines = Resources.Load<TextAsset>("Environment/tree").text;
    }
}
