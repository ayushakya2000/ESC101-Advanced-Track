using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamAdvanced : MonoBehaviour
{
    public static float speed = 40;
    public static float angle = 0;
    public static double fixangle = 30;
    // Start is called before the first frame update

    public static bool moveUp = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moveUp)
        {
            if (angle < fixangle)
            {
                angle += speed * Time.deltaTime;
                transform.Rotate(-speed * Time.deltaTime, 0, 0);
            }
        }
        else
        {
            if(angle>0)
            {
                angle -= speed * Time.deltaTime;
                transform.Rotate(speed * Time.deltaTime, 0, 0);
            }
        }
    }

    void moveCame(bool k)
    {

    }
}
