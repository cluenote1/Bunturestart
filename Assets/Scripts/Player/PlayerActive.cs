using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActive : MonoBehaviour
{
    private Rigidbody2D rigid;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip wireSound;
    [SerializeField] private WireController wire; // WireController 대신 WireController2D로 변경

    [SerializeField] private float jumpPower = 25f;
    private float currentJumpPower;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>(); // Rigidbody2D 사용
    }

    private void Start()
    {
        currentJumpPower = jumpPower;
    }

    public void Jump()
    {
        rigid.velocity = Vector2.up * currentJumpPower; // Vector3를 Vector2로 변경
        SoundManager.Instance.SFXPlay("Jump", jumpSound);
    }

    public void JumpShot()
    {
        wire.ShotWire();
    }

    public void OnWire()
    {
        SoundManager.Instance.SFXPlay("Wire", wireSound);
        StartCoroutine("Wire");
    }

    private void WireJump()
    {
        if (DataManager.Instance.Stage < 2)
        {
            rigid.gravityScale = 1; // Rigidbody2D에서 gravityScale 사용
            rigid.velocity = Vector2.up * jumpPower * 1.3f;
        }
        else
        {
            StartCoroutine("WireJumping");
        }
    }

    private IEnumerator Wire()
    {
        rigid.gravityScale = 0; // Rigidbody2D에서 gravityScale 사용
        float transY = transform.position.y;
        float veloY = 0;
        float gra = 9.8f;
        float graScale = 14;
        float rev = 20f + wire.WireDistance * 1.2f;
        veloY -= rev;
        do
        {
            veloY += gra * graScale * Time.deltaTime;
            transform.position += new Vector3(0, veloY * Time.deltaTime, 0); // Vector3를 Vector2로 변경
            yield return null;
        }
        while (transY > transform.position.y);
        WireJump();
        wire.DisableWire();
    }

    private IEnumerator WireJumping()
    {
        while (transform.position.y < 7.5f)
        {
            transform.position += new Vector3(0, jumpPower * Time.deltaTime, 0); // Vector3를 Vector2로 변경
            yield return null;
        }
        rigid.gravityScale = 1; // Rigidbody2D에서 gravityScale 사용
        rigid.velocity = Vector2.up * 6f;
    }
}
