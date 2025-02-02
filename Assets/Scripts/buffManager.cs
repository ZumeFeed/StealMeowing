using UnityEngine;
using UnityEngine.UI;

public class BuffManager : MonoBehaviour
{

    [SerializeField] float buffTime = 4;
    float buffTimer;

    SpriteRenderer buffFrame;
    Image buffBarImage;
    HatControl hatControl;

    public void StartBuffTimer()
    {
        hatControl.SetIsBuffed(true);
        buffBarImage.enabled = true;
        buffFrame.enabled = true;
        buffTimer = buffTime;
        buffBarImage.fillAmount = buffTimer / buffTime;
    }

    void Start()
    {
        buffBarImage = GetComponent<Image>();
        buffFrame = GameObject.Find("buffFrame").GetComponent<SpriteRenderer>();
        hatControl = GameObject.Find("Hat").GetComponent<HatControl>();
        buffBarImage.enabled = false;
        buffFrame.enabled = false;
    }

    void Update()
    {
        if (buffTimer > 0)
        {
            buffTimer -= Time.deltaTime;
            buffBarImage.fillAmount = buffTimer / buffTime;
            if (buffTimer <= 0)
            {
                buffTimer = 0;
                buffBarImage.enabled = false;
                buffFrame.enabled = false;
                hatControl.StopMoving();
                hatControl.SetIsBuffed(false);
            }
        }
    }
}
