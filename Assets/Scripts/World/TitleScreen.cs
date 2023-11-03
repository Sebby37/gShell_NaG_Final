using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public string gameScene;
    public string menuScene;
    public string linuxUrl;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(menuScene);
    }

    public void LearnMore()
    {
        Application.OpenURL(linuxUrl);
    }
}
