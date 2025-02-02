using UnityEngine;

public class dogSpawn : MonoBehaviour
{
    Vector2 startPosition, startDogPosition, endPosition;
    [SerializeField] float timeMoving = 4f;
    [SerializeField] float dogTimeMoving = 2f;
    float timeElapsed = 0;
    GameObject hat;
    GameObject mainCamera;
    GameObject dog;
    SoundControl soundControl;

    bool isAnimation = false;
    bool isBackward = false;
    bool isDogRun = false;

    void Start()
    {
        hat = GameObject.Find("Hat");
        mainCamera = GameObject.Find("Main Camera");
        dog = GameObject.Find("dog");
        soundControl = GameObject.Find("SoundListener").GetComponent<SoundControl>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        startPosition = mainCamera.transform.position;
        endPosition = new Vector3(-10,3.25f);
        hat.GetComponent<HatControl>().SetIsStuned(true);
        isAnimation = true;
        Debug.Log("Началась анимация появления собаки");
    }

    private void Update()
    {
        if (isAnimation)
        {
            if (!isBackward)
            {
                timeElapsed += Time.deltaTime;
                mainCamera.transform.position = (Vector3)(startPosition + (endPosition - startPosition) * (timeElapsed / timeMoving)) + new Vector3(0,0,-10);
                if (timeElapsed >= timeMoving)
                {
                    timeElapsed = 0;
                    isAnimation = false;
                    isDogRun = true;
                    startDogPosition = dog.transform.position;
                }
            }
            else
            {
                timeElapsed -= Time.deltaTime;
                if (timeElapsed < 0)
                {
                    dog.GetComponent<DogRun>().StartRun();
                    isAnimation = false;
                    dog.GetComponent<Animator>().SetBool("isBarks", false);
                    mainCamera.transform.position = hat.transform.position;
                    hat.GetComponent<HatControl>().SetIsStuned(false);
                }
                mainCamera.transform.position = (Vector3)(startPosition + (endPosition - startPosition) * (timeElapsed / dogTimeMoving)) + new Vector3(0, 0, -10);
            }
        }

        if (isDogRun)
        {
            timeElapsed += Time.deltaTime;

            dog.transform.position = Vector3.Lerp(startDogPosition,endPosition,timeElapsed/dogTimeMoving);
            if (timeElapsed >= dogTimeMoving)
            {
                dog.GetComponent<Animator>().SetBool("isBarks",true);
                soundControl.playSound(SoundControl.audioName.dogBark);
                isDogRun = false;
                timeElapsed = dogTimeMoving;
            }
        }
    }

    public void animTrigger()
    {
        isAnimation = true;
        isBackward = true;
    }

}
