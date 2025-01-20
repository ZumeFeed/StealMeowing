using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] List<GameObject> clouds;
    [SerializeField] float cloudsSpeed = -0.05f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (var cloud in clouds)
        {
            cloud.transform.position += new Vector3(cloudsSpeed, 0);
            if (cloud.transform.position.x + 5 <= -5)
                cloud.transform.position = new Vector3(10,cloud.transform.position.y);
        }
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
