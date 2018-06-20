using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour {
    public enum GunMode { AR, SG, SR, RPG, MISSILE }

    [SerializeField]
    private Animator animator;

    public Animator GetAnimator { get; private set; }
    
    void Start()
    {
        GetAnimator = animator;
    }

    // Update is called once per frame
    void Update ()
    {
        if (GameDirector.Instance.LocalPlayerName != gameObject.name)
        {
            return;
        }
        if (GetComponent<PlayerController>().state != PlayerController.State.Flying)
        {
            animator.SetFloat("Vertical", GetComponent<PlayerController>().Vertical);
            animator.SetFloat("Horizontal", GetComponent<PlayerController>().Horizontal);
            animator.SetBool("IsJump", GetComponent<PlayerController>().state == PlayerController.State.Jump);
            animator.SetBool("Booster", GetComponent<PlayerController>().isBooster);
        }
        else
        {
            animator.SetFloat("Vertical", 0);
            animator.SetFloat("Horizontal", 0);
            animator.SetBool("IsJump", false);
            animator.SetBool("Booster", false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool("Aim", true);
        }

        if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("Aim", false);
        }
    }
}
