using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blink_PressToStart : MonoBehaviour
{
    private float time;
    private Image image;
    private float blinkSpeed = 0.3f; //±ôºýÀÌ´Â ½Ã°£ Á¶Àý, ³·À»¼ö·Ï °£°Ý ±æ¾îÁü

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if(time < 0.5f)
        {
            image.color = new Color(1, 1, 1, 1 - time);
        }
        else
        {
            image.color = new Color(1, 1, 1, time);
            if(time > 1f)
            {
                time = 0;
            }
        }
        time += Time.deltaTime * blinkSpeed;
    }
}
