using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
// 완성본
public class PlayerController : NetworkBehaviour {
	public enum State { Normal, Flying, Jump }
	enum Force { walkForceMax = 3, flyForceMax = 20, gravity = -50, jumpForceMax = 1000 }

	Rigidbody rgbd;
	Vector3 dir;
    
	[SerializeField]
	private GameObject camCenter;

	[SerializeField]
	private Camera tpsCam;

    [SerializeField]
    private Camera fpsCam;

    // 지면과 얼마나 떨어졌는지 확인을 위한 ray start
    [SerializeField]
	private GameObject rayStart;

	[SerializeField]
	private PlayerStats playerStats;
    // UI
	[SerializeField]
	private PlayerUi playerUi;

    // 허리, 상하 시점 조절을 위한
    [SerializeField]
    private GameObject Spine;

    private float SpineAngle = 0f;

    Force force;	

	public float camAxisX = 0;
	
	float walkForce = (float)Force.walkForceMax;
	float flyForce = (float)Force.flyForceMax;
	float addFly = 0f;
	float addFlyForward = (float)Force.flyForceMax;
	float jumpForce = (float)Force.jumpForceMax;
	
	public State state { get; private set; }

	private bool freeView = false;

	public bool isFPS { get; private set; }
	public bool isUp { get; private set; }
	public bool isDown { get; private set; }

	private bool isJump = false;  

	private float flyPosY;
	private bool isAltitudeHolding = false;
	
	public float Vertical { get; private set; }
	public float Horizontal { get; private set; }
    public float MouseWheel { get; private set; }
    public bool IsJump { get; private set; }

	Vector3 vel = Vector3.zero;

    public bool isBooster { get; private set; }

    public float SpineXAngle;

    // Use this for initialization
    void Start () {
		rgbd = this.GetComponent<Rigidbody>();
		dir = Vector3.zero;
		state = State.Normal;
		isUp = false;
		isDown = false;
		isFPS = false;
		Vertical = 0f;
		Horizontal = 0f;
        isBooster = false;
        IsJump = isJump;        
    }

    // Update is called once per frame
    void Update () {
        if (!Cursor.visible)
        {
            MoveController();
            ViewPoint();
        }
    }

    void LateUpdate()
    {
        if (Vertical == 0 && Horizontal == 0)
        {
            Spine.transform.localEulerAngles = Spine.transform.localEulerAngles + new Vector3(SpineXAngle, 0f, -camAxisX);
        }
        else
        {
            Spine.transform.localEulerAngles = Spine.transform.localEulerAngles + new Vector3(0f, 0f, -camAxisX);
        }
    }

    // 물리 관련 연산
    void FixedUpdate()
	{
        if (!Cursor.visible)
        {
            Movement();
        }
	}

	private void ViewPoint()
	{
		// 1, 3인칭 변환
		if (Input.GetKeyDown(KeyCode.C))
		{
			isFPS = !isFPS;
			if (isFPS)
			{
                fpsCam.enabled = true;
				playerUi.backCamMask.SetActive(true);
				playerUi.miniCamMask.SetActive(true);

                // 추가 파트
                foreach (GameObject item in playerUi.invenBG)
                {
                    item.SetActive(true);
                }
            }
			else
			{
                fpsCam.enabled = false;
                playerUi.backCamMask.SetActive(false);
				playerUi.miniCamMask.SetActive(false);

                // 추가 파트
                foreach (GameObject item in playerUi.invenBG)
                {
                    item.SetActive(false);
                }
            }
		}

        // aimCircle 회전
        if (playerUi.aimCircle != null)
        {
            playerUi.aimCircle.transform.Rotate(0, 0, -Input.GetAxis("Mouse X") * 0.25f);
        }

        // 자유 시점 변환
        if (Input.GetKeyDown(KeyCode.G))
		{
			freeView = true;
		}
		if (Input.GetKeyUp(KeyCode.G))
		{
			freeView = false;
		}

        // 좌우 회전
        camAxisX -= Input.GetAxis("Mouse Y");

        MouseWheel = Input.GetAxis("Mouse ScrollWheel");

        if (camAxisX > 50f)
        {
            camAxisX = 50f;
        }
        else if (camAxisX < -50f)
        {
            camAxisX = -50f;
        }

        // 자유 시점 변환이 아닐 때 시점 변환
        if (!Cursor.visible)
        {
            if (!freeView)
            {
                float rotateSpeed = 1f;

                if (GetComponent<PlayerAttack>().IsZoom)
                {
                    if (isFPS)
                    {
                        rotateSpeed = (1 - ((90f - GetComponent<PlayerAttack>().current_fps_zoom) / 90f) * 0.9f);
                    }
                    else
                    {
                        rotateSpeed = (1 - ((90f - GetComponent<PlayerAttack>().current_tps_zoom) / 90f) * 0.9f);
                    }
                }

                // 상하
                this.transform.Rotate(0f, Input.GetAxis("Mouse X") * rotateSpeed, 0f);
                camCenter.transform.eulerAngles = new Vector3(camAxisX * rotateSpeed, transform.eulerAngles.y, 0f);
            }
            else
            {
                camCenter.transform.Rotate(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f);
            }
        }
    }

	private void MoveController()
	{
		// 비행 중지
		if (Input.GetKeyDown(KeyCode.H))
		{
			if (state == State.Flying)
			{
				state = State.Normal;
			}
		}
		
		// WSAD 입력
		Vertical = Input.GetAxis("Vertical");
		Horizontal = Input.GetAxis("Horizontal");


		// 상승
		if (Input.GetKeyDown(KeyCode.B))
		{
			isUp = true;
		}

		if (Input.GetKeyUp(KeyCode.B))
		{
			isUp = false;
		}

		// 하강
		if (Input.GetKeyDown(KeyCode.V))
		{
			isDown = true;
		}
		if (Input.GetKeyUp(KeyCode.V))
		{
			isDown = false;
		}

		// 점프
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (state == State.Normal)
			{
				isJump = true;
			}
		}
		if (Input.GetKeyUp(KeyCode.Space))
		{
			isJump = false;
		}

		// 부스트
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			walkForce = (float)Force.walkForceMax * 2f;
			flyForce = (float)Force.flyForceMax * 2f;
			addFlyForward = (float)Force.flyForceMax * 3f;
            isBooster = true;
            fpsCam.transform.localPosition = new Vector3(0f, 0f, 0.1f);
        }
		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			walkForce = (float)Force.walkForceMax;
			flyForce = (float)Force.flyForceMax;
			addFlyForward = (float)Force.flyForceMax;
            isBooster = false;
            fpsCam.transform.localPosition = Vector3.zero;
        }
	}

	// 점프가 끝나는 것을 인지
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Block" && state == State.Jump)
		{
			state = State.Normal;

            GetComponent<PlayerSound>().PlayOneShot(gameObject.name, (int)PlayerSound.AudioClipNum.JUMP, 1f);
        }
	}

	// 에너지를 다 쓰면 비행 종료
	public void FlyExit()
	{
		state = State.Normal;
		addFly = 0;
	}

	// 이름대로
	private void Movement()
	{
		vel = rgbd.velocity;

		// 상승
		if (isUp == true && !playerStats.IsEnegyZero())
		{
			state = State.Flying;   // 비행 중

			isAltitudeHolding = false;  // 고도 유지 취소
			addFly += 5f;

			if (addFly > flyForce)
			{
				addFly = flyForce;
			}
		}

		// 비행 중 이라면
		if (state == State.Flying)
		{
			// 하강
			if (isDown && !playerStats.IsEnegyZero())
			{
				addFly -= 5f;
				isAltitudeHolding = false;
				if (addFly < -flyForce)
				{
					addFly = -flyForce;
				}
			}

			// 고도 유지
			if (isAltitudeHolding)
			{
				if (flyPosY > transform.position.y)
				{
					addFly += 1f;
					if (addFly > 10f)
					{
						addFly = 10f;
					}
				}
				else
				{
					addFly -= 1f;
					if (addFly < 5f)
					{
						addFly = 5f;
					}
				}
            }
			if (!isUp && !isDown && Mathf.Abs(vel.y) > 3f)
			{				
                rgbd.velocity = new Vector3(vel.x, vel.y * 0.99f, vel.z);

				flyPosY = transform.position.y; // 고도 유지할 좌표
				isAltitudeHolding = true;
			}
		}

		// 비행 중
		if (state == State.Flying)
		{
			dir = (transform.forward * Vertical + transform.right * Horizontal) * addFlyForward
			+ transform.up * (addFly - (float)Force.gravity);
			rgbd.AddForce(dir);
            
			if (!isUp)
			{
				Vector3 pos = rayStart.transform.position;
				Ray ray = new Ray(pos, Vector3.down);
				RaycastHit hit;
				
				if (Physics.Raycast(ray, out hit, 1f))
				{
					if (hit.collider.gameObject.tag == "Block")
					{
						state = State.Normal;
						addFly = 0;
					}
				}
			}
        }
		// 비행 중이 아닐 때, 지면에서
		else
		{
            if (isJump)
			{
				if (state != State.Jump)
				{
                    state = State.Jump;
					isJump = false;
					jumpForce = (float)Force.jumpForceMax;
					dir = transform.up * jumpForce;
					rgbd.AddForce(dir);
				}
			}			

			if (Vertical == 0 && Horizontal == 0)
			{
				rgbd.velocity = rgbd.transform.up * vel.y;
			}
			else
			{
				rgbd.velocity = (transform.forward * Vertical + transform.right * Horizontal) * walkForce
								+ rgbd.transform.up * vel.y;
			}	
		}

		// 중력
		dir = transform.up * (float)Force.gravity;
		rgbd.AddForce(dir);      
    }
}