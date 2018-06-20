using UnityEngine;
using UnityEngine.Networking;

public class PlayerMoveSound : NetworkBehaviour
{
    private PlayerSound playerSound;

    private bool isMove = false;
    private bool isFly = false;

    private bool isBoost = false;

    private void Start()
    {
        playerSound = GetComponent<PlayerSound>();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isBoost = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isBoost = false;
        }

         if ((GetComponent<PlayerController>().Vertical != 0 || GetComponent<PlayerController>().Horizontal != 0)
                && (GetComponent<PlayerController>().state == PlayerController.State.Normal))
         {
            if (!isMove)
            {
                if (isBoost)
                {
                    isMove = true;
                    playerSound.Play(gameObject.name, (int)PlayerSound.AudioClipNum.RUN, 1f);
                }
                else
                {
                    isMove = true;
                    playerSound.Play(gameObject.name, (int)PlayerSound.AudioClipNum.WALK, 1f);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    playerSound.Play(gameObject.name, (int)PlayerSound.AudioClipNum.RUN, 1f);
                }
                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    playerSound.Play(gameObject.name, (int)PlayerSound.AudioClipNum.WALK, 1f);
                }
            }
         }
        
        if (GetComponent<PlayerController>().state == PlayerController.State.Flying)
        {
            if (!isFly)
            {
                playerSound.Play(gameObject.name, (int)PlayerSound.AudioClipNum.FLY, 0.2f);
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    playerSound.Play(gameObject.name, (int)PlayerSound.AudioClipNum.FLY, 1f);
                }
                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    playerSound.Play(gameObject.name, (int)PlayerSound.AudioClipNum.FLY, 0.2f);
                }
            }
            isMove = false;
            isFly = true;
        }

        if (GetComponent<PlayerController>().state == PlayerController.State.Normal)
        {
            if (isFly)
            {
                playerSound.Stop();
            }
            isFly = false;
        }
        if (GetComponent<PlayerController>().Vertical == 0 && GetComponent<PlayerController>().Horizontal == 0
            && (GetComponent<PlayerController>().state == PlayerController.State.Normal))
        {
            isMove = false;
            playerSound.Stop();
        }
    }
}
