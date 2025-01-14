using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minicat_run : MonoBehaviour
{

    public float speed = 1.2f;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, 0);
    }
}
