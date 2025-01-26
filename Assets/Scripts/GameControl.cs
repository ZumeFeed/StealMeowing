using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    HatCollisionHandler hatCH;

    [SerializeField] float offsetTime = 2;
    float offsetTimer = 0;


    void Start()
    {
        hatCH = GameObject.Find("Hat").GetComponent<HatCollisionHandler>();
    }

    // Update is called once per frame
    void Update()
    {

        if (hatCH.getIsHatFall())
        {
            offsetTimer += Time.deltaTime;
            if (offsetTimer >= offsetTime)
            {
                if (hatCH.getIsFinishedLevel())
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
