using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onButtonPlayClick()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void onButtonLevelsClick()
    {
        SceneManager.LoadScene("Level Selection");
    }

    public void onButtonLevel2Click()
    {
        SceneManager.LoadScene("Level 2");
    }
    public void onButtonLevel3Click()
    {
        SceneManager.LoadScene("Level 3");
    }
    public void onButtonBackClick()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
