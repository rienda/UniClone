using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneManager : MonoBehaviour
{    
    int pascolorstate = 0;
    int precolorstate = 0;

    float elasped = 0.0f;

    Color curColor;
    Color color_0;
    Color color_1 = Color.red;
    Color color_2 = new Color(255 / 255, 0, 255 / 255);
    Color color_3 = Color.blue;
    Color color_4 = new Color(0, 255 / 255, 255 / 255);
    Color color_5 = Color.green;
    Color color_6 = new Color(255 / 255, 255 / 255, 0);    

    void Start()
    {
        color_0 = Color.white;
    }

    void Update()
    {
        ChangeColor();
        CheckColor();
        ColorstateCheck();      
    }

    void ChangeColor()
    {      
        elasped += Time.deltaTime;

        if (precolorstate == 0)
        {
            curColor = Color.Lerp(color_0, color_1, elasped / 3.0f);
                     
            GetComponent<Image>().color = curColor;
        }
        if (precolorstate == 1)
        {
            curColor = Color.Lerp(color_1, color_2, elasped / 1.0f);

            GetComponent<Image>().color = curColor;
        }
        if (precolorstate == 2)
        {
            curColor = Color.Lerp(color_2, color_3, elasped / 1.0f);

            GetComponent<Image>().color = curColor;
        }
        if (precolorstate == 3)
        {
            curColor = Color.Lerp(color_3, color_4, elasped / 1.0f);

            GetComponent<Image>().color = curColor;
        }
        if (precolorstate == 4)
        {
            curColor = Color.Lerp(color_4, color_5, elasped / 1.0f);

            GetComponent<Image>().color = curColor;
        }
        if (precolorstate == 5)
        {
            curColor = Color.Lerp(color_5, color_6, elasped / 1.0f);

            GetComponent<Image>().color = curColor;
        }
        if (precolorstate == 6)
        {
            curColor = Color.Lerp(color_6, color_1, elasped / 1.0f);

            GetComponent<Image>().color = curColor;
        }
    }

    void CheckColor()
    {
        if(GetComponent<Image>().color == color_0)
        {
            precolorstate = 0;
        }
        if (GetComponent<Image>().color == color_1)
        {
            precolorstate = 1;
        }
        if (GetComponent<Image>().color == color_2)
        {
            precolorstate = 2;
        }
        if (GetComponent<Image>().color == color_3)
        {
            precolorstate = 3;
        }
        if (GetComponent<Image>().color == color_4)
        {
            precolorstate = 4;
        }
        if (GetComponent<Image>().color == color_5)
        {
            precolorstate = 5;
        }
        if (GetComponent<Image>().color == color_6)
        {
            precolorstate = 6;
        }
    }

    void ColorstateCheck()
    {
       if(pascolorstate != precolorstate)
        {
            elasped = 0f;
            pascolorstate = precolorstate;
        }
    }
}
