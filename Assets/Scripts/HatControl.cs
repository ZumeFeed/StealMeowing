using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HatControl : MonoBehaviour
{
    Vector2 contorlVector, moveDirect;
    float moveDistance;
    LineRenderer line, arrow;
    public Vector2 getMoveDirect() { return moveDirect; }

    [SerializeField] float maxMovingDistance = 3, timeMovingBase = 0.05f;
    float timeMoving = 0, timeMovingElapsed = 0;
    [SerializeField] int timeMovingDivider = 8;
    Vector2 startPosition, endPosition;
 
    bool isMove = false, isStuned = false;
    public bool getIsMove() { return isMove; }
    public void SetIsStuned(bool state) { isStuned = state; }

    Rigidbody2D rb;
    Animator animator;
    SoundControl soundControl;

    float afkTimer = 0, afkTime = 15;

    void MoveCalculate() // Просчет направления и силы движения по вектору (Позиция мыши - Шляпа)
    {
        contorlVector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        moveDistance = contorlVector.magnitude;
        moveDirect = contorlVector / moveDistance;
        if (moveDistance > maxMovingDistance)
            moveDistance = maxMovingDistance;
    }

    public void TurnOffArrow()
    {
        line.enabled = false;
        arrow.enabled = false;
    }

    public void StopMoving()
    {
        isMove = false;
        timeMovingElapsed = 0;
    }

    void Start()
    {
        line = GameObject.Find("line").GetComponent<LineRenderer>();
        arrow = GameObject.Find("arrow").GetComponent<LineRenderer>();
        soundControl = GameObject.Find("SoundListener").GetComponent<SoundControl>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isMove && !isStuned) // Управление, доступно если не двигается и не в стане
        {
            if (Input.GetMouseButton(0)) // Левая кнопка мыши зажата
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

            if (Input.GetMouseButtonUp(0)) // Левая кнопка мыши отпущена после зажатия
            {
                MoveCalculate();
                TurnOffArrow();

                timeMoving = timeMovingBase + (moveDistance / timeMovingDivider); // Рассчет времени передвижения, по формуле (Базовое время + (Расстояние / делительВремени))
                startPosition = transform.position;
                endPosition = transform.position - (Vector3)(moveDistance * moveDirect);

                isMove = true; // Начало движения

                if (moveDirect.x >= 0) 
                    animator.SetInteger("state", 4);
                else 
                    animator.SetInteger("state", 3); // Анимация в зависимости от направление

                soundControl.playSound(SoundControl.audioName.hatMove);
            }
        }

        if (isMove) // Движение
        {
            timeMovingElapsed += Time.deltaTime;
            rb.position = Vector3.Lerp(startPosition, endPosition, timeMovingElapsed/timeMoving); // Просчитанное движение по вектору
            if (timeMovingElapsed >= timeMoving)
            {
                StopMoving(); // Конец движения
                animator.SetInteger("state", 0);
            }
        }

        if (!isMove && !isStuned) // Таймер Afk анимации
            afkTimer += Time.deltaTime;
        else
            afkTimer = 0;

        if (afkTimer >= afkTime)
        {
            animator.SetInteger("state", 1);
        }
    }

    public void afkAnimationTrigger()
    {
        animator.SetInteger("state", 0);
    }

}
