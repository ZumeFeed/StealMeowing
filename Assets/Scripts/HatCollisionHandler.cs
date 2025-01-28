using System.Collections.Generic;
using UnityEngine;

public class HatCollisionHandler : MonoBehaviour
{

    [SerializeField] sbyte health = 3;
    [SerializeField] List<Sprite> hpSprites;
    GameObject healthBar;
    GameObject finish;

    [SerializeField] float stunTime = 0.2f, blinkPeriod = 0.1f;
    float stunTimer = 0, blinkTimer = 0;
    bool isBlink = false;

    int observers = 0;
    bool isStuned = false, isHatFall = false, finishedLevel = false;
    public bool getIsHatFall() { return isHatFall; }
    public bool getIsFinishedLevel() { return finishedLevel; }

    SoundControl soundControl;
    Animator animator;
    HatControl hatControl;
    BuffManager buffManager;
    Rigidbody2D rb;

    [SerializeField] GameObject miniCat;

    void EndLevel(bool finished) // true если финиш
    {
        animator.SetInteger("state", 6); // Переход в тригер fallHat
        isHatFall = true;
        hatControl.SetIsStuned(true);
        if (hatControl.getMoveDirect().x < 0)
            GetComponent<SpriteRenderer>().flipX = true;

        this.finishedLevel = finished;
    }

    void Start()
    {
        soundControl = GameObject.Find("SoundListener").GetComponent<SoundControl>();
        healthBar = GameObject.Find("HealthBar");
        finish = GameObject.Find("Bucket");
        hatControl = GetComponent<HatControl>();
        buffManager = GameObject.Find("buffBar").GetComponent<BuffManager>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isStuned)
        {
            stunTimer += Time.deltaTime;
            blinkTimer += Time.deltaTime;

            if (stunTimer >= stunTime)
            {
                isStuned = false;
                hatControl.SetIsStuned(false);

                stunTimer = 0;
                isBlink = false;
                blinkTimer = 0;
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                animator.SetInteger("state", 0);
            }

            if (blinkTimer >= blinkPeriod)
            {
                isBlink = !isBlink;
                blinkTimer = 0;
            }

            if (isBlink)
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.2f);
            else
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }

    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        hatControl.TurnOffArrow();
        hatControl.StopMoving();
        rb.position += hatControl.getMoveDirect() / 6;

        if (collision2D.gameObject == finish)
        {
            EndLevel(true);
            finish.GetComponent<Animator>().SetBool("stealFish", true);
            soundControl.playSound(SoundControl.audioName.hitBucket);
        }
        else
        {
            hatControl.SetIsStuned(true);
            animator.SetInteger("state", 5); // Переход в триггер Hit
            soundControl.playSound(SoundControl.audioName.hitWall);
        }
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.tag == "Observation")
            observers++;
        if (collider2D.tag == "Buff")
        {
            buffManager.StartBuffTimer();
            Destroy(collider2D.gameObject);
            Debug.Log("Баф подобран");
        }

    }
    void OnTriggerStay2D(Collider2D collider2D)
    {
        if (collider2D.tag == "Observation")
        {
            if (collider2D.GetComponent<ScareMech>().getIsWatching() && hatControl.GetIsMove())
            {
                hatControl.SetIsStuned(true);
                hatControl.StopMoving();
                animator.SetInteger("state", 5); // Переход в триггер Hit
                soundControl.playSound(SoundControl.audioName.scarecrowScream);
            }
        }
    }
    void OnTriggerExit2D(Collider2D collider2D)
    {
        if (collider2D.tag == "Observation")
            observers--;
    }

    public void HitAnimationTrigger()
    {
        health--;
        healthBar.GetComponent<SpriteRenderer>().sprite = hpSprites[health];

        if (health == 0)
        {
            EndLevel(false);
        }
        else
        {
            isStuned = true;
        }
    }
    public void fallHatAnimationTrigger()
    {
        if (hatControl.getMoveDirect().x < 0)
            miniCat = Instantiate(miniCat, transform);
        else
        {
            miniCat = Instantiate(miniCat, transform);
            miniCat.GetComponent<minicat_run>().speed = -(miniCat.GetComponent<minicat_run>().speed);
            miniCat.GetComponent<SpriteRenderer>().flipX = true;
        }
        if (finishedLevel)
            miniCat.GetComponent<Animator>().SetBool("isFish", true);

        soundControl.playSound(SoundControl.audioName.catMeowing);
    }
}
