using UnityEngine;

public class GravityCore : MonoBehaviour
{
    private GameObject target;
    private bool isHooked = false;
    private Vector3 hookedVecter;

    public bool IsHooked { get; private set; }
    public Vector3 HookedVector { get; private set; }
    
    void OnCollisionEnter(Collision other)
    {
        target = other.gameObject;

        ContactPoint contact = other.contacts[0];
        //Debug.DrawRay(contact.point, contact.normal * 10f, Color.red, 10f);
        //contact.point : 충돌 지점, contact.normal : 충돌한 지점의 수직 벡터, 색상은 빨강, 10초 유지

        if (target != null && target.gameObject.tag == "Block")
        {
            IsHooked = true;
            HookedVector = contact.normal;
        }
    }
}
