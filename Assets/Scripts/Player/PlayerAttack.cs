using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPunCallbacks
{
    [SerializeField] float forceAmount;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        { // 특정 입력을 받을 때
            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.forward, out hit, float.MaxValue))
            { // 눈앞에 물체가 있는지 판별
                Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), transform.forward * float.MaxValue, Color.red);
                Debug.Log(hit.collider.name);
                CharacterController cc = hit.transform.GetComponent<CharacterController>();
                if (cc != null)
                { // 물체에 CharacterController가 있는 경우
                    Vector3 direction = hit.transform.position - transform.position; // 플레이어와 물체 사이의 방향 계산
                    direction = direction.normalized; // 방향을 저장

                    // PunRPC를 사용하여 네트워크에 연결된 모든 클라이언트에게 이 메서드를 호출하도록 요청
                    photonView.RPC("ApplyForce", RpcTarget.All, hit.transform.GetComponent<PhotonView>().ViewID, direction, forceAmount);
                }
            }
        }
    }

    // PunRPC 어트리뷰트를 사용하여 이 메서드를 RPC 메서드로 만듭니다.
    [PunRPC]
    void ApplyForce(int viewID, Vector3 direction, float force)
    {
        // 네트워크에 연결된 모든 클라이언트에서 해당 물체를 찾음
        CharacterController cc = PhotonView.Find(viewID).GetComponent<CharacterController>();

        // 힘을 가해야 할 때
        if (force > 0)
        {
            // 힘의 양만큼 이동
            cc.Move(direction * force * Time.deltaTime);
            // 힘의 양을 줄임 (매 프레임마다 힘의 양을 줄여서 점점 멈추게 함)
            force -= Time.deltaTime * 100; // 100은 임의의 값으로, 필요에 따라 조절하세요.
            if (force < 0) force = 0; // 힘의 양이 0보다 작아지지 않도록 함
        }
    }
}
