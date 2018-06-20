using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GravityCoreController : NetworkBehaviour
{
    [SerializeField]
    private GameObject corePrefab;

    [SerializeField]
    private GameObject shootStart;

    [SerializeField]
    private GameObject cam;

    [SerializeField]
    private float coreAddForce = 20f;

    GameObject core = null;

    Rigidbody coreRgbd;
    Rigidbody userRgbd;

    Ray ray;

    Vector3 origin;
    Vector3 direction;
    Vector3 stopPos;

    private float checkedMoveTime = 0.0f;

    private bool isUserMove = false;
    private bool isCore = false;
    private bool isShootCore = false;

    void Awake()
    {
        userRgbd = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        ShootCore();
        UserBreak();
        CoreHookCheck();
        CheckedTime();
    }
    //[Client]
    private void ShootCore()
    {
        if (!isCore && core == null)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                CreateCore();
            }
        }
    }
    // 코어 생성을 위한 레이 생성
    //[Client]
    private void CreateCore()
    {
        isCore = true;
        isShootCore = true;
        ray = new Ray(shootStart.transform.position, cam.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2000f))
        {
            ray = new Ray(shootStart.transform.position,
                        (hit.point - shootStart.transform.position));
            //Debug.DrawRay(ray.origin, ray.direction * 20f, Color.blue, 3.0f);
        }
        origin = ray.origin;
        direction = ray.direction;
        ShootCore(origin, direction);
    }

    // 레이의 정보를 받아서 코어 생성  
    public void ShootCore(Vector3 origin, Vector3 direction)
    {
        core = Instantiate(corePrefab, origin, Quaternion.identity);
        coreRgbd = core.GetComponent<Rigidbody>();
        coreRgbd.velocity = Vector3.zero;
        coreRgbd.AddForce(direction.normalized * coreAddForce, ForceMode.Impulse);
        
        // 일정 시간 후 생성된 코어 삭제 하기위한 체크
        StartCoroutine(ResetCall());
    }

    // 일정 시간 후 생성된 코어 삭제 하기위한 체크
    IEnumerator ResetCall()
    {
        yield return new WaitForSeconds(3f);

        if (core != null)
            Reset();
    }

    // 코어 삭제 
    private void Reset()
    {
        if (core != null)
        {
            coreRgbd.velocity = Vector3.zero;

            Destroy(core);
            core = null;
        }
        isCore = false;
    }

    // 유저가 코어로 이동할 때 
    private void CheckedTime()
    {
        if (isUserMove == true)
        {
            checkedMoveTime += Time.deltaTime;
            //userRgbd.useGravity = false;
            //userRgbd.isKinematic = true;
            userRgbd.position = Vector3.Lerp(userRgbd.position, direction, Time.deltaTime * 4.0f);
        }
    }

    // 코어가 오브젝트에 닿았는지
    private void CoreHookCheck()
    {
        if ((core != null && core.GetComponent<GravityCore>().IsHooked) && isShootCore == true)
        {
            userRgbd.velocity = Vector3.zero;
            coreRgbd.velocity = Vector3.zero;
            stopPos = userRgbd.position;
            direction = coreRgbd.position;
            coreRgbd.transform.localScale = new Vector3(0.8f, 0.8f, 0.05f);
            coreRgbd.transform.rotation = Quaternion.LookRotation(core.GetComponent<GravityCore>().HookedVector);
            isUserMove = true;
            isShootCore = false;
        }
    }

    // 유저가 코어에 일정시간 다가가거나 거의 도달했을 때
    private void UserBreak()
    {
        if (checkedMoveTime > 1.2f || Vector3.Distance(userRgbd.position, direction) <= 1.0f)
        {
            //Debug.Log("브레이크");
            //userRgbd.velocity = Vector3.zero;
            //userRgbd.isKinematic = false;
            //direction = userRgbd.transform.up * 30.0f;
            //userRgbd.AddForce(direction, ForceMode.Impulse);
            //userRgbd.useGravity = true;
            //isUserMove = false;
            checkedMoveTime = 0.0f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 취소
        userRgbd.velocity = Vector3.zero;
        isUserMove = false;
    }
}