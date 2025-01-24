using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HatControl : MonoBehaviour
{
    Vector2 contorlVector, moveDirect;
    float moveDistance;
    LineRenderer line, arrow;

    [SerializeField] float maxMovingDistance = 3, timeMovingBase = 0.05f;
    float timeMoving = 0, timeMovingElapsed = 0;
    [SerializeField] int timeMovingDivider = 8;
    Vector2 startPosition, endPosition;
 
    bool isMove = false, isStuned = false;
    public void SetIsMove(bool state) { isMove = state; }
    public void SetIsStuned(bool state) { isStuned = state; }

    Rigidbody2D rb;
    Animator animator;
    SoundControl soundControl;

    float afkTimer = 0, afkTime = 15;

    void Start()
    {
        line = GameObject.Find("line").GetComponent<LineRenderer>();
        arrow = GameObject.Find("arrow").GetComponent<LineRenderer>();
        soundControl = GameObject.Find("SoundListener").GetComponent<SoundControl>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void MoveCalculate() // ������� ����������� � ���� �������� �� ������� (������� ���� - �����)
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

    void Update()
    {
        if (!isMove && !isStuned) // ����������, �������� ���� �� ��������� � �� � �����
        {
            if (Input.GetMouseButton(0)) // ����� ������ ���� ������
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

            if (Input.GetMouseButtonUp(0)) // ����� ������ ���� �������� ����� �������
            {
                MoveCalculate();
                TurnOffArrow();

                timeMoving = timeMovingBase + (moveDistance / timeMovingDivider); // ������� ������� ������������, �� ������� (������� ����� + (���������� / ���������������))
                startPosition = transform.position;
                endPosition = transform.position - (Vector3)(moveDistance * moveDirect);

                isMove = true; // ������ ��������

                if (moveDirect.x >= 0) 
                    animator.SetInteger("state", 4);
                else 
                    animator.SetInteger("state", 3); // �������� � ����������� �� �����������

                soundControl.playSound(SoundControl.audioName.hatMove);
            }
        }

        if (isMove) // ��������
        {
            timeMovingElapsed += Time.deltaTime;
            rb.position = Vector3.Lerp(startPosition, endPosition, timeMovingElapsed/timeMoving); // ������������ �������� �� �������
            if (timeMovingElapsed >= timeMoving)
            {
                StopMoving(); // ����� ��������
                animator.SetInteger("state", 0);
            }
        }

        if (!isMove && !isStuned) // ������ Afk ��������
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
