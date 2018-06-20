using UnityEngine;

public class EmpZone : MonoBehaviour
{
    private GameObject target;

    void OnTriggerStay(Collider other)
    {
        if (other != null)
        {
            target = other.gameObject;
            if (target != null && target.gameObject.tag == "Player")
            {
                GameDirector.Instance.players[target.name].GetComponent<PlayerStats>().InsideEmpZone();
            }
        }
    }
}
