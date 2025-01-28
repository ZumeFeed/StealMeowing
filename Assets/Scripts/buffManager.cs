using UnityEngine;
using UnityEngine.UI;

public class BuffManager : MonoBehaviour
{

    [SerializeField] float buffTime = 4;
    float buffTimer;

    Image buffBarImage;
    HatControl hatControl;

    public void StartBuffTimer()
    {
        hatControl.SetIsBuffed(true);
        buffTimer = buffTime;
        buffBarImage.enabled = true;
        buffBarImage.fillAmount = buffTimer / buffTime;
    }

    void Start()
    {
        buffBarImage = GetComponent<Image>();
        buffBarImage.enabled = false;
        hatControl = GameObject.Find("Hat").GetComponent<HatControl>();
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
                hatControl.SetIsBuffed(false);
            }
        }
    }
}
