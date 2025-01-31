using UnityEngine;

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

    [SerializeField] float runSpeedBase = 0.0005f;

    bool isMove = false, isStuned = false, isBuffed = false;
    public bool GetIsMove() { return isMove; }
    public void SetIsStuned(bool state) { isStuned = state; }
    public bool GetIsBuffed() { return isBuffed; }
    public void SetIsBuffed(bool isBuffed) { this.isBuffed = isBuffed; }

    Rigidbody2D rb;
    Animator animator;
    SoundControl soundControl;

    float afkTimer = 0, afkTime = 15;

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

    public void TurnOnArrow()
    {
        line.enabled = true;
        line.SetPosition(0, transform.position + (Vector3)(moveDirect * moveDistance));
        line.SetPosition(1, transform.position);
        arrow.enabled = true;
        arrow.SetPosition(0, transform.position - (Vector3)moveDirect / 2);
        arrow.SetPosition(1, transform.position - (Vector3)moveDirect * 0.75f);
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
        if (!isMove && !isStuned && !isBuffed) // ����������, �������� ���� �� ��������� � �� � �����
        {
            if (Input.GetMouseButton(0)) // ����� ������ ���� ������
            {
                MoveCalculate();
                TurnOnArrow();

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
        else if (isBuffed && !isStuned)
        {
            if (Input.GetMouseButton(0))
            {
                MoveCalculate();
                TurnOnArrow();

                rb.position -= runSpeedBase * moveDirect * moveDistance * Time.deltaTime;
                animator.SetInteger("state", 7);
            }
            if (Input.GetMouseButtonUp(0)) // ����� ������ ���� �������� ����� �������
            {
                TurnOffArrow();
                animator.SetInteger("state", 0);
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
