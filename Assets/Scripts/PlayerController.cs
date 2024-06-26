using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;

    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _rigidbody.velocity *= 0.5f;
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        _rigidbody.velocity = new Vector3(h, 0, v).normalized * speed;
        SetRotate();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.State != EGameState.InGame)
            return;
        var pos = Vector3ByteConversion.Vector3ToBytes(transform.position);
        var rot = Vector3ByteConversion.Vector3ToBytes(transform.rotation.eulerAngles);
        var msg = new byte[24];
        Buffer.BlockCopy(pos, 0, msg, 0, pos.Length);
        Buffer.BlockCopy(rot, 0, msg, pos.Length, rot.Length);
        NetworkManager.Instance.Send(new Message(EMessageType.MT_USER_ACTION, msg));
    }

    void SetRotate()
    {
        Vector3 mousePosition = Input.mousePosition;
        // 마우스 위치를 월드 좌표로 변환
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.transform.position.y));

        // 현재 오브젝트 위치
        Vector3 objectPosition = transform.position;

        // 오브젝트에서 마우스 위치로의 방향 벡터
        Vector3 direction = mousePosition - objectPosition;
        direction.y = 0; // y축 방향은 무시

        // 방향 벡터로부터 회전 각도를 계산
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        // 오브젝트의 y축 회전을 설정
        transform.rotation = Quaternion.Euler(0, -angle + 90, 0);
    }

}
