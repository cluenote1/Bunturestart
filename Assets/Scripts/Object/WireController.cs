using System.Collections;
using UnityEngine;

public class WireController : MonoBehaviour
{
    [SerializeField] private Transform playerHand;
    [SerializeField] private Vector2 shotDirection;
    [SerializeField] private float shotSpeed = 10f;
    [SerializeField] private float maxDistance = 9f;
    [SerializeField] private AudioClip shotSound;
    [SerializeField] private LayerMask wireBuilding;

    private float wireDistance;
    public float WireDistance => wireDistance;

    private bool onWire;
    public bool OnWire => onWire;

    private LineRenderer line;
    private Transform DRB;
    private RaycastHit2D hit;
    private bool isWire;

    private void Awake()
    {
        DRB = transform.GetChild(0);
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true; // Ensure LineRenderer is in world space
    }

    private void Start()
    {
        DisableWire();
    }

    private void Update()
    {
        if (line.enabled)
        {
            line.SetPosition(0, playerHand.position);
            line.SetPosition(1, transform.position);
        }
    }

    public void ShotWire()
    {
        if (isWire) return;
        SoundManager.Instance.SFXPlay("ShotWire", shotSound);
        isWire = true;
        DRB.gameObject.SetActive(true);
        line.enabled = true;
        StartCoroutine(Shot());
    }

    public void DisableWire()
    {
        onWire = false;
        isWire = false;
        DRB.gameObject.SetActive(false);
        line.enabled = false;
        transform.parent = null;
    }

    private IEnumerator Shot()
    {
        Vector2 lastHandPos = playerHand.position;
        transform.position = lastHandPos;
        Vector2 targetPos = (shotDirection.normalized * maxDistance);
        wireDistance = Vector2.Distance(transform.position, targetPos + lastHandPos);

        while (wireDistance > 0f)
        {
            wireDistance = Vector2.Distance(transform.position, targetPos + lastHandPos);
            transform.position = Vector2.MoveTowards(transform.position, targetPos + lastHandPos, Time.deltaTime * shotSpeed);

            RaycastHit2D hit = Physics2D.Raycast(lastHandPos, shotDirection, maxDistance);
            if (hit.collider != null && (wireBuilding & (1 << hit.collider.gameObject.layer)) != 0)
            {
                Target(hit);
                break;
            }
            yield return null;
        }

        if (wireDistance == 0f)
            DisableWire();
    }

    private void Target(RaycastHit2D hit)
    {
        transform.parent = hit.transform;
        onWire = true;
    }
}
