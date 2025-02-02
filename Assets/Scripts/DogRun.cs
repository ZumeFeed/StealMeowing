using UnityEngine;

public class DogRun : MonoBehaviour
{

    [SerializeField] float speed = 0.02f;
    bool isRun = false;
    [SerializeField] GameObject region;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (isRun)
        {
            transform.position += new Vector3 (speed, 0);
        }
    }

    public void StartRun()
    {
        isRun = true;
    }

    public void StopRun()
    {
        isRun = false;
    }

    void isDogBurksAnimationTrigger()
    {
        region.GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("dogSpawnTrigger").GetComponent<dogSpawn>().animTrigger();
    }
}
