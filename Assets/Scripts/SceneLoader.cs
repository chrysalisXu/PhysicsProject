using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        // SceneManager.LoadScene(1);
        // SceneManager.LoadScene("FlagAndWind");
        // SceneManager.LoadScene("Plastic");
    }

    // Update is called once per frame
    void Update()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Press ESC to quit
            Application.Quit();
            print("ESC Quit!");
        }
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitTheGame()
    {
        Application.Quit();
        print("Quit!");
    }

}
