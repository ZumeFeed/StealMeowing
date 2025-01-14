using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareMech : MonoBehaviour
{

    bool isWatching = false; 
    public bool getIsWatching() 
    { return isWatching; }


    [SerializeField] float watchTime = 1, sleepTime = 1;
    float timer = 0;

    [SerializeField] List<Sprite> scareSprite;

    // Update is called once per frame
    void Update()
    {
        if (isWatching)
        {
            timer += Time.deltaTime;
            if (timer >= watchTime)
            {
                isWatching = !isWatching;
                timer = 0;
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= sleepTime)
            {
                isWatching = !isWatching;
                timer = 0;
            }
        }

        if (isWatching)
        {
            transform.parent.GetComponent<SpriteRenderer>().sprite = scareSprite[1];
            GetComponent<SpriteRenderer>().color = new Color32(0, 15, 85, 100);
        }
        else
        {
            transform.parent.GetComponent<SpriteRenderer>().sprite = scareSprite[0];
            GetComponent<SpriteRenderer>().color = new Color32(0, 15, 85, 10);
        }

    }
}
