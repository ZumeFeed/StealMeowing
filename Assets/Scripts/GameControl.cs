using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    HatControl hatControl;

    [SerializeField] float offsetTime = 2;
    float offsetTimer = 0;

    [SerializeField] GameObject miniCat;

    void Start()
    {
        hatControl = GameObject.Find("Hat").GetComponent<HatControl>();
    }

    // Update is called once per frame
    void Update()
    {

        if (offsetTimer > 0)
        {
            offsetTimer -= Time.deltaTime;
            if (offsetTimer <= 0)
            {
                if (nextlevelTrigger)
                {
                    if (SceneManager.GetActiveScene().buildIndex == 3)
                        SceneManager.LoadScene(1);
                    else
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }
    }
}
