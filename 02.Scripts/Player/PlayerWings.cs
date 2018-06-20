using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerWings : NetworkBehaviour {
	public enum State { Normal, Flying, Jump }

	[SerializeField]
	private List<ParticleSystem> afterBurners;

    public List<ParticleSystem> AfterBurners { get; private set; }

    [SerializeField]
	private List<GameObject> wings;

	[SerializeField]
	private PlayerController playerController;

	[SerializeField]
	private PlayerStats playerStats;

	private const float afterBurnerSpeedIdle = 0.01f;
	private const float afterBurnerSpeedDefault = 0.1f;
	private const float afterBurnerSpeedUp = 0.2f;
	private const float afterBurnerSpeedBoost = 0.5f;

	private float afterBurnerSpeed = 0.2f;

	State state;

    private bool isStop = false;

    private void Start()
    {
        AfterBurners = afterBurners;
    }

    // Update is called once per frame
    void Update () {
		if (!isLocalPlayer)
		{
			return;
		}
        if (playerController != null)
        {
            state = (State)playerController.state;
            WingController();
            wingsRotation();
        }
	}

    [Client]
	void WingController()
	{
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            afterBurnerSpeed = afterBurnerSpeedBoost;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            afterBurnerSpeed = afterBurnerSpeedUp;
        }

        if (state == State.Flying)
		{
            isStop = false;

            if (afterBurners[0].isStopped)
			{
                CmdFlyEffectPlay(transform.name);
            }

			if (playerController.isUp || playerController.isDown)
			{
                CmdStartLifetime(transform.name, 2, 3, afterBurnerSpeed);
            }
			else
			{
                CmdStartLifetime(transform.name, 2, 3, afterBurnerSpeedDefault);
            }

			if (playerController.Vertical != 0 || playerController.Horizontal != 0)
			{
                CmdStartLifetime(transform.name, 0, 1, afterBurnerSpeed);
            }
			else
			{
                CmdStartLifetime(transform.name, 0, 1, afterBurnerSpeedIdle);
            }
		}

		if (playerStats.IsEnegyZero() || state == State.Normal && !isStop)
        {
            isStop = true;
            CmdFlyEffectStop(transform.name);
        }
	}

    [Command]
    private void CmdFlyEffectPlay(string playerName)
    {
        RpcFlyEffectPlay(playerName);
    }

    [ClientRpc]
    private void RpcFlyEffectPlay(string playerName)
    {
        foreach (var item in GameDirector.Instance.players[playerName].GetComponent<PlayerWings>().AfterBurners)
        {
            item.gameObject.SetActive(true);
            item.Play(true);
        }
    }

    [Command]
    private void CmdFlyEffectStop(string playerName)
    {
        RpcFlyEffectStop(playerName);
    }

    [ClientRpc]
    private void RpcFlyEffectStop(string playerName)
    {
        foreach (ParticleSystem item in GameDirector.Instance.players[playerName].GetComponent<PlayerWings>().AfterBurners)
        {
            item.gameObject.SetActive(false);
            item.Stop(true);
        }
    }

    [Command]
    private void CmdStartLifetime(string playerName, int idx1, int idx2, float Speed)
    {
        RpcStartLifetime(playerName, idx1, idx2, Speed);
    }

    [ClientRpc]
    private void RpcStartLifetime(string playerName, int idx1, int idx2, float Speed)
    {
        GameDirector.Instance.players[playerName].GetComponent<PlayerWings>().AfterBurners[idx1].startLifetime = Speed;
        GameDirector.Instance.players[playerName].GetComponent<PlayerWings>().AfterBurners[idx2].startLifetime = Speed;
    }


    void wingsRotation()
	{
		if (state == State.Normal || state == State.Jump)
		{
			wings[0].transform.localEulerAngles = new Vector3(180f, 0f, 10f);
			wings[1].transform.localEulerAngles = new Vector3(180f, 0f, -10f);
			wings[2].transform.localEulerAngles = new Vector3(180f, 0f, 0f);
			wings[3].transform.localEulerAngles = new Vector3(180f, 0f, 0f);
		}
		else
		{
			if (playerController.Vertical == 0 && playerController.Horizontal == 0)
			{
				wings[0].transform.localEulerAngles = new Vector3(0f, 0f, 45f);
				wings[1].transform.localEulerAngles = new Vector3(0f, 0f, -45f);
			}
			else if (playerController.Vertical > 0 && playerController.Horizontal == 0)
			{
				wings[0].transform.localEulerAngles = new Vector3(0f, 0f, 45f);
				wings[1].transform.localEulerAngles = new Vector3(0f, 0f, -45f);
			}
			else if (playerController.Vertical < 0 && playerController.Horizontal == 0)
			{
				wings[0].transform.localEulerAngles = new Vector3(0f, 180f, -45f);
				wings[1].transform.localEulerAngles = new Vector3(0f, 180f, 45f);
			}
			else if (playerController.Vertical == 0 && playerController.Horizontal > 0)
			{
				wings[0].transform.localEulerAngles = new Vector3(-45f, 90f, 0f);
				wings[1].transform.localEulerAngles = new Vector3(45f, 90f, 0f);
			}
			else if (playerController.Vertical == 0 && playerController.Horizontal < 0)
			{
				wings[0].transform.localEulerAngles = new Vector3(45f, -90f, 0f);
				wings[1].transform.localEulerAngles = new Vector3(-45f, -90f, 0f);
			}
			else if (playerController.Vertical > 0 && playerController.Horizontal > 0)
			{
				wings[0].transform.localEulerAngles = new Vector3(-45f, 90f, 0f);
				wings[1].transform.localEulerAngles = new Vector3(0f, 0f, -45f);
			}
			else if (playerController.Vertical > 0 && playerController.Horizontal < 0)
			{
				wings[0].transform.localEulerAngles = new Vector3(0f, 0f, 45f);
				wings[1].transform.localEulerAngles = new Vector3(-45f, -90f, 0f);
			}
			else if (playerController.Vertical < 0 && playerController.Horizontal > 0)
			{
				wings[0].transform.localEulerAngles = new Vector3(0f, 180f, -45f);
				wings[1].transform.localEulerAngles = new Vector3(45f, 90f, 0f);
			}
			else if (playerController.Vertical < 0 && playerController.Horizontal < 0)
			{
				wings[0].transform.localEulerAngles = new Vector3(45f, -90f, 0f);
				wings[1].transform.localEulerAngles = new Vector3(0f, 180f, 45f);
			}

			if (!playerController.isDown)
			{
				wings[2].transform.localEulerAngles = new Vector3(-90f, 0f, 30f);
				wings[3].transform.localEulerAngles = new Vector3(-90f, 0f, -30f);
			}
			else
			{
				wings[2].transform.localEulerAngles = new Vector3(90f, 180f, -30f);
				wings[3].transform.localEulerAngles = new Vector3(90f, 180f, 30f);
			}
		}
	}

}
