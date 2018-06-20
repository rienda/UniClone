using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadCamera : MonoBehaviour
{
    public float Vertical { get; private set; }
    public float Horizontal { get; private set; }
    public float MouseY { get; private set; }
    public float camAxisX { get; private set; }

    bool isMouseLock = true;


    void Start()
    {
        Vertical = 0f;
        Horizontal = 0f;
        MouseY = 0f;
        camAxisX = 0f;
    }

    void Update()
    {
        // 유체이탈캠 움직임
        if (GameDirector.Instance.GetComponent<GameStart>().GetStart == false || GameDirector.Instance.isPlayerDead)
        {
            CheckMouseLook();
            if (isMouseLock)
            {
                DeadCamMove();
            }
        }            

        // 게임 시작후 뎁스값 낮추기
        if(GameDirector.Instance.GetComponent<GameStart>().GetStart)
        {
            GetComponent<Camera>().depth = -5;
        }       
        
        // 플레이어 죽은후 카메라에 달린 죽음 상태 UI 온
        if(GameDirector.Instance.isPlayerDead)
        {
            // TODO : 지훈이 형이 작업한 레이어 온 시켜주기
        }
    }

    void DeadCamMove()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Vertical = 1f;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            Vertical = 0f;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Vertical = -1f;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            Vertical = 0f;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Horizontal = -1f;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Horizontal = 1f;
        }

        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
        {
            Horizontal = 0f;
        }

        transform.position = transform.position + transform.forward * Vertical + transform.right * Horizontal;
                
        float rotateSpeed = 1f;

        camAxisX -= Input.GetAxis("Mouse Y");
        
        if (camAxisX > 50f)
        {
            camAxisX = 50f;
        }
        else if (camAxisX < -60f)
        {
            camAxisX = -60f;
        }
        
        // 좌우
        transform.Rotate(0f, Input.GetAxis("Mouse X") * rotateSpeed, 0f);
        // 상하
        transform.eulerAngles = new Vector3(camAxisX * rotateSpeed, transform.eulerAngles.y, 0f);
    }

    void CheckMouseLook()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isMouseLock = !isMouseLock;
        }

        // 마우스가 잠겨 있다면
        if (isMouseLock)
        {
            // 말 그대로 1
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;   // 커서를 화면 중앙에 고정
        }
        else
        {
            // 말 그대로 2
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;     // 고정 해제
        }
    }
}
