using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour {

    private float rotationX = 0f;
    
    void FixedUpdate()
    {
        rotationX += Time.fixedDeltaTime * 2f;
        if (rotationX >= 360f)
        {
            rotationX = 0f;
        }
        transform.eulerAngles = new Vector3(rotationX, 0f, 0f);
    }
}
