using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotator : MonoBehaviour
{
    public static float speed=135;
    public static float angle=0;
    public static double fixangle = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (angle < fixangle)
        {
            angle += speed * Time.deltaTime;
            transform.Rotate(0, speed * Time.deltaTime, 0);
        }
        else if(fixangle!=double.PositiveInfinity)
        {
            Quaternion q = Quaternion.Euler(new Vector3(0, (float)fixangle, 0));
            transform.rotation = q;//gives exact multiple of 180 in camera...
        }
    }
}
