using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private WireController wire;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private AudioClip startSound;

    private PlayerSprite playerSprite;
    private PlayerActive playerActive;
    private Rigidbody2D rigid;

    private Transform currentHand;
    private Vector3 jumpHandPos;
    private Vector3 wireHandPos;

    private bool isJump = false;
    private bool isDead;
    private bool isWire;

    private void Awake()
    {
        playerSprite = transform.Find("PlayerSprite").GetComponent<PlayerSprite>();
        playerActive = GetComponent<PlayerActive>();
        rigid = GetComponent<Rigidbody2D>();
        currentHand = transform.Find("CurrentHandPosition").transform;
        jumpHandPos = transform.Find("JumpHandPosition").transform.localPosition;
        wireHandPos = transform.Find("WireHandPosition").transform.localPosition;
    }

    private void Start()
    {
        SoundManager.Instance.SFXPlay("Start", startSound);
    }

    private void Update()
    {
        if (DataManager.Instance.isDead)
        {
            if (isDead)
                return;
            rigid.velocity = new Vector2(rigid.velocity.x, 10f); // 2D에서는 y축만 사용
            GetComponent<Collider2D>().enabled = false;
            transform.position = new Vector3(transform.position.x, transform.position.y, -1);
            playerSprite.SetAnimation(PlayerAnimation.Dead);
            isDead = true;
            return;
        }

        if (InputManager.Instance.GetJumpKey())
        {
            if (OnGrounded())
            {
                currentHand.localPosition = jumpHandPos;
                StartCoroutine(Jumping());
                playerActive.Jump();
                playerSprite.SetAnimation(PlayerAnimation.Jump);
            }
            else
            {
                playerActive.JumpShot();
                playerSprite.SetAnimation(PlayerAnimation.JumpShot);
            }
        }
        if (wire.OnWire && !isWire)
        {
            currentHand.localPosition = wireHandPos;
            isWire = true;
            playerActive.OnWire();
            playerSprite.SetAnimation(PlayerAnimation.Wire);
        }
        else if (wire.OnWire)
        {
            transform.up = (wire.transform.position - transform.position).normalized;
        }
        else if (!wire.OnWire && isWire)
        {
            transform.rotation = Quaternion.identity;
            isWire = false;
            playerSprite.SetAnimation(PlayerAnimation.Rolling);
        }
        if (OnGrounded() && !isJump)
        {
            playerSprite.SetAnimation(PlayerAnimation.Run);
            if (DeadCast() && !wire.OnWire)
            {
                gameManager.GameOver();
            }
        }
    }

    private IEnumerator Jumping()
    {
        isJump = true;
        yield return new WaitForSeconds(0.1f);
        isJump = false;
    }

    private bool OnGrounded()
    {
        // 2D에서 BoxCast를 사용하려면 Collider2D와 Physics2D.OverlapBox를 사용
        return Physics2D.OverlapBox((Vector2)transform.position + new Vector2(0, -0.1f), new Vector2(1.5f, 0.2f), 0f, jumpableGround);
    }

    private bool DeadCast()
    {
        // 2D에서 Raycast를 사용하려면 Physics2D.Raycast를 사용
        return !Physics2D.Raycast((Vector2)transform.position + new Vector2(-0.25f, 0), Vector2.down, 4f, jumpableGround);
    }
}
