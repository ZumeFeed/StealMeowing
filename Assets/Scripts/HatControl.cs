using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HatControl : MonoBehaviour
{
    Vector3 mousePos;
    Vector2 vector, direct;
    float distance;

    [SerializeField] float maxDistance = 3;
    [SerializeField] float timeMovingBase = 0.05f;
    float timeMoving = 0, timeElapsed = 0;

    Vector2 startPosition, endPosition;
    LineRenderer line, arrow;

    bool isMove;
    sbyte observers = 0;
    [SerializeField] int div = 8;

    [SerializeField] float stunTime = 0.2f;
    [SerializeField] float blinkPeriod = 0.1f;
    float stunTimer = 0;
    float blinkTimer = 0;
    bool blink = false;
    bool isStuned = false;
    bool isHatFall = false;
    bool nextlevel = false;

    [SerializeField] sbyte health = 3;
    [SerializeField] GameObject healthBar;
    [SerializeField] List<Sprite> hpSprites;
    [SerializeField] List<Sprite> bucketSprites;

    [SerializeField] GameObject miniCat;

    float afkTimer = 0, afkTime = 15;

    float timer = 0;
    [SerializeField] float offsetTime = 2;

    Rigidbody2D rb;
    Animator animator;

    AudioSource audioSource;
    [SerializeField] List<AudioClip> sounds;

    void Start()
    {
        line = GameObject.Find("line").GetComponent<LineRenderer>();
        arrow = GameObject.Find("arrow").GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        audioSource = GameObject.Find("SoundListener").GetComponent<AudioSource>();
    }

    void Update()
    {

        if (Input.GetMouseButton(0) && !isMove && !isStuned && !isHatFall)
        {
            vector = mousePos - transform.position;
            distance = vector.magnitude; // найти расстояние между ними
            direct = vector / distance; // найти направление

            if (distance > maxDistance) distance = maxDistance;

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            line.enabled = true;
            line.SetPosition(0, transform.position + new Vector3(direct.x * distance, direct.y * distance));
            line.SetPosition(1, transform.position);

            arrow.enabled = true;
            arrow.SetPosition(0, transform.position - new Vector3(direct.x/2, direct.y/2, 0));
            arrow.SetPosition(1, transform.position - new Vector3(direct.x * (distance / 4) + direct.x/2, direct.y * (distance / 4) + direct.y/2, 0));

            animator.SetInteger("state", 2);
            afkTimer = 0;
        }

        if (Input.GetMouseButtonUp(0) && !isMove && !isStuned && !isHatFall)
        {
            vector = mousePos - transform.position;
            distance = vector.magnitude; // найти расстояние между ними
            direct = vector / distance; // найти направление

            line.enabled = false;
            arrow.enabled = false;
        
            if (distance > maxDistance) distance = maxDistance;
            timeMoving = timeMovingBase + (distance / div);

            startPosition = transform.position;
            endPosition = new Vector3(transform.position.x - ((distance) * direct.x), transform.position.y - ((distance) * direct.y));

            isMove = true;

            if (direct.x >= 0)
                animator.SetInteger("state", 4);
            else
                animator.SetInteger("state", 3);

            audioSource.clip = sounds[0];
            audioSource.Play();
        }

        if (isMove)
        {
            timeElapsed += Time.deltaTime;
            rb.position = Vector3.Lerp(startPosition, endPosition, timeElapsed/timeMoving);
            if (timeElapsed >= timeMoving)
            {
                timeElapsed = 0;
                isMove = false;
                animator.SetInteger("state", 0);
            }
        }

        if (isStuned)
        {

            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkPeriod)
            {
                blink = !blink;
                blinkTimer = 0;
            }

            if (blink) 
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.2f);
            else
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

            stunTimer += Time.deltaTime;
            if (stunTimer >= stunTime)
            {
                animator.SetInteger("state", 0);
                isStuned = false;
                stunTimer = 0;
                blink = false;
                blinkTimer = 0;
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <=0)
            {
                if (nextlevel)
                {
                    if (SceneManager.GetActiveScene().buildIndex == 3)
                        SceneManager.LoadScene(1);
                    else
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }


        if (!isMove && !isStuned)
            afkTimer += Time.deltaTime;
        else
            afkTimer = 0;

        if (afkTimer >= afkTime)
        {
            animator.SetInteger("state", 1);
        }

        
    }
    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.transform.tag == "Finish")
        {
            collision2D.transform.GetComponent<SpriteRenderer>().sprite = bucketSprites[1];
            isMove = false;
            timeElapsed = 0;
            rb.position += direct / 6;
            animator.SetInteger("state", 6);
            isHatFall = true;
            nextlevel = true;
            if (direct.x < 0)
                GetComponent<SpriteRenderer>().flipX = true;

            audioSource.clip = sounds[2];
            audioSource.Play();
        }
        else if (isMove)
        {
            isMove = false;
            timeElapsed = 0;
            rb.position += direct / 6;
            animator.SetInteger("state", 5);

            audioSource.clip = sounds[1];
            audioSource.Play();
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
                isMove = false; //Заглушка
                timeElapsed = 0;
                animator.SetInteger("state", 5);

                audioSource.clip = sounds[3];
                audioSource.Play();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        if (collider2D.tag == "Observation")
            observers--;
    }

    public void afkAnimationTrigger()
    {
        animator.SetInteger("state", 0);
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
            timer = offsetTime;
            if (direct.x < 0)
                GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            isStuned = true;
        }
    }

    public void fallHatAnimationTrigger()
    {
        if (direct.x < 0)
        {
            miniCat = Instantiate(miniCat, transform);
        }
        else if (direct.x >= 0)
        {
            miniCat = Instantiate(miniCat, transform);
            miniCat.GetComponent<minicat_run>().speed = -miniCat.GetComponent<minicat_run>().speed;
            miniCat.GetComponent<SpriteRenderer>().flipX = true;
        }
        timer = offsetTime;

        if (nextlevel)
        {
            miniCat.GetComponent<Animator>().SetBool("isFish", true);
        }

        audioSource.clip = sounds[4];
        audioSource.Play();
    }

}
