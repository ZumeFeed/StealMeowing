using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HatCollisionHandler : MonoBehaviour
{
    HatControl hatControl;

    [SerializeField] float stunTime = 0.2f;
    [SerializeField] float blinkPeriod = 0.1f;
    float stunTimer = 0;
    float blinkTimer = 0;

    [SerializeField] sbyte health = 3;
    [SerializeField] GameObject healthBar;
    [SerializeField] List<Sprite> hpSprites;
    [SerializeField] List<Sprite> bucketSprites;

    bool isBlink = false;
    bool nextlevelTrigger = false;

    bool isMove = false, isStuned = false, isHatFall = false;

    SoundControl soundControl;


    void Start()
    {
        hatControl = GameObject.Find("Hat").GetComponent<HatControl>();
        soundControl = GameObject.Find("SoundListener").GetComponent<SoundControl>();
    }

    void Update()
    {

        if (isStuned)
        {

            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkPeriod)
            {
                isBlink = !isBlink;
                blinkTimer = 0;
            }

            if (isBlink)
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.2f);
            else
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

            stunTimer += Time.deltaTime;
            if (stunTimer >= stunTime)
            {
                isStuned = false;
                stunTimer = 0;
                isBlink = false;
                blinkTimer = 0;
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                animator.SetInteger("state", 0);
            }
        }

    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        hatControl.TurnOffArrow();

        if (collision2D.transform.tag == "Finish")
        {
            collision2D.transform.GetComponent<SpriteRenderer>().sprite = bucketSprites[1]; // Анимация ведра
            hatControl.StopMoving();
            rb.position += moveDirect / 6;
            animator.SetInteger("state", 6);
            isHatFall = true;
            nextlevelTrigger = true;
            if (moveDirect.x < 0)
                GetComponent<SpriteRenderer>().flipX = true;

            soundControl.playSound(SoundControl.audioName.hitBucket);
        }
        else
        {
            isMove = false;
            timeMovingElapsed = 0;
            rb.position += moveDirect / 6;
            animator.SetInteger("state", 5);

            soundControl.playSound(SoundControl.audioName.hitWall);
        }
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.tag == "Observation")
            observers++;
    }
    void OnTriggerStay2D(Collider2D collider2D)
    {
        if (collider2D.tag == "Observation")
        {
            if (collider2D.GetComponent<ScareMech>().getIsWatching() && isMove)
            {
                isMove = false;
                timeMovingElapsed = 0;
                animator.SetInteger("state", 5);

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
        //if (observers == 0)
        health--;
        //else
        //    health-=2;
        //if (health < 0)
        //    health = 0;

        healthBar.GetComponent<SpriteRenderer>().sprite = hpSprites[health];
        if (health == 0)
        {
            animator.SetInteger("state", 6);
            isHatFall = true;
            offsetTimer = offsetTime;
            if (moveDirect.x < 0)
                GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            isStuned = true;
        }
    }
    public void fallHatAnimationTrigger()
    {
        if (moveDirect.x < 0)
        {
            miniCat = Instantiate(miniCat, transform);
        }
        else if (moveDirect.x >= 0)
        {
            miniCat = Instantiate(miniCat, transform);
            miniCat.GetComponent<minicat_run>().speed = -miniCat.GetComponent<minicat_run>().speed;
            miniCat.GetComponent<SpriteRenderer>().flipX = true;
        }
        offsetTimer = offsetTime;

        if (nextlevelTrigger)
        {
            miniCat.GetComponent<Animator>().SetBool("isFish", true);
        }

        soundControl.playSound(SoundControl.audioName.catMeowing);
    }
}
