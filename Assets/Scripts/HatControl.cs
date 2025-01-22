using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HatControl : MonoBehaviour
{
    Vector3 mousePosition;
    Vector2 contorlVector, moveDirect;
    float moveDistance;
    LineRenderer line, arrow;

    [SerializeField] float maxDistance = 3, timeMovingBase = 0.05f;
    float timeMoving = 0, timeMovingElapsed = 0;
    Vector2 startPosition, endPosition;

    bool isMove;
    bool isBlink = false;
    bool isStuned = false;
    bool isHatFall = false;
    bool nextlevel = false;

    sbyte observers = 0;
    [SerializeField] int div = 8; // Плохое именование

    [SerializeField] float stunTime = 0.2f;
    [SerializeField] float blinkPeriod = 0.1f;
    float stunTimer = 0;
    float blinkTimer = 0;

    [SerializeField] sbyte health = 3;
    [SerializeField] GameObject healthBar;
    [SerializeField] List<Sprite> hpSprites; // Плохо что должен иметь ссылку
    [SerializeField] List<Sprite> bucketSprites;

    [SerializeField] GameObject miniCat;

    float afkTimer = 0, afkTime = 15;

    [SerializeField] float offsetTime = 2;
    float offsetTimer = 0;

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

    void MoveCalculate()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        contorlVector = mousePosition - transform.position;
        moveDistance = contorlVector.magnitude;
        moveDirect = contorlVector / moveDistance;
        if (moveDistance > maxDistance) 
            moveDistance = maxDistance;
    }

    void Update()
    {
        if (!isMove && !isStuned && !isHatFall) // Управление
        {
            if (Input.GetMouseButton(0))
            {
                MoveCalculate();

                line.enabled = true;
                line.SetPosition(0, transform.position + (Vector3)(moveDirect * moveDistance));
                line.SetPosition(1, transform.position);
                arrow.enabled = true;
                arrow.SetPosition(0, transform.position - (Vector3)moveDirect / 2);
                arrow.SetPosition(1, transform.position - (Vector3)moveDirect * 0.75f);

                afkTimer = 0;
                animator.SetInteger("state", 2);
            }

            if (Input.GetMouseButtonUp(0))
            {
                MoveCalculate();

                line.enabled = false;
                arrow.enabled = false;

                timeMoving = timeMovingBase + (moveDistance / div);
                startPosition = transform.position;
                endPosition = transform.position - (Vector3)(moveDistance * moveDirect);

                isMove = true;

                if (moveDirect.x >= 0) 
                    animator.SetInteger("state", 4);
                else 
                    animator.SetInteger("state", 3);

                audioSource.clip = sounds[0];
                audioSource.Play();
            }
        }

        if (isMove) // Движение
        {
            timeMovingElapsed += Time.deltaTime;
            rb.position = Vector3.Lerp(startPosition, endPosition, timeMovingElapsed/timeMoving);
            if (timeMovingElapsed >= timeMoving)
            {
                timeMovingElapsed = 0;
                isMove = false;
                animator.SetInteger("state", 0);
            }
        }

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

        if (offsetTimer > 0)
        {
            offsetTimer -= Time.deltaTime;
            if (offsetTimer <=0)
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
        line.enabled = false;
        arrow.enabled = false;

        if (collision2D.transform.tag == "Finish")
        {
            collision2D.transform.GetComponent<SpriteRenderer>().sprite = bucketSprites[1];
            isMove = false;
            timeMovingElapsed = 0;
            rb.position += moveDirect / 6;
            animator.SetInteger("state", 6);
            isHatFall = true;
            nextlevel = true;
            if (moveDirect.x < 0)
                GetComponent<SpriteRenderer>().flipX = true;

            audioSource.clip = sounds[2];
            audioSource.Play();
        }
        else if (isMove)
        {
            isMove = false;
            timeMovingElapsed = 0;
            rb.position += moveDirect / 6;
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
                timeMovingElapsed = 0;
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

        if (nextlevel)
        {
            miniCat.GetComponent<Animator>().SetBool("isFish", true);
        }

        audioSource.clip = sounds[4];
        audioSource.Play();
    }

}
