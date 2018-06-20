using UnityEngine;
using UnityEngine.Networking;

public class EmpExplosion : NetworkBehaviour {
    [SerializeField]
    private AudioClip[] audioClips;

    [SerializeField]
    private AudioSource audioSource;

    // 이펙트
    [SerializeField]
    private GameObject explosion;
    // 공중에서 떨어지는 폭탄
    [SerializeField]
    private GameObject bomb;

    void OnCollisionEnter(Collision other)
    {
        GameObject particle = Instantiate(explosion, bomb.transform.position, Quaternion.identity);

        audioSource.PlayOneShot(audioClips[0]);
        CheckPlayerInRange(transform.position);
        Destroy(particle, 3f);
        bomb.SetActive(false);
    }

    // 폭탄 범위 내부인지 체크 
    private void CheckPlayerInRange(Vector3 position)
    {
        // 범위안의 모든 플레이어 받아오기
        Collider[] colliders = Physics.OverlapSphere(position, 10f, 1 << LayerMask.NameToLayer("Player"));

        // 플레이어가 장애물 뒤에 있는지 체크 후 아니면 에너지 0 
        foreach (Collider player in colliders)
        {
            RaycastHit hit;
            if (Physics.Raycast(position, player.transform.position - position, out hit, 10f,
                1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Block")))
            {
                if (hit.transform.gameObject.tag == "Player")
                {
                    GameDirector.Instance.players[hit.transform.gameObject.name].GetComponent<PlayerStats>().TouchedEmpBomb();
                }
            }
        }
    }
}
