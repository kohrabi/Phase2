
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class GridMoveComponent : MonoBehaviour
{
    public const float RowSize = 0.5f;
    public const float ColumnSize = 0.5f;
    public const float MoveSpeed = 8f;
    public const float MoveDelay = 0.2f;
    public static bool Moved = false;
    public static bool CanMove = true;
    public static Coroutine DelayMoveCoroutine = null;
    private new Rigidbody2D rigidbody;
    private new BoxCollider2D collider;
    public Vector3 Velocity = Vector3.zero;

    Vector3 moveCurr = Vector3.zero; // Dung Xoa
    public Vector3 MoveTarget = Vector3.zero; // Dung Xoa
    //RaycastHit2D[] castResults = new RaycastHit2D[7];

    // Start is called before the first frame update
    void Awake()
    {
        transform.position = new Vector3(
                                    Mathf.RoundToInt(transform.position.x / RowSize) * RowSize, 
                                    Mathf.RoundToInt(transform.position.y / ColumnSize) * ColumnSize, 
                                    0);
        moveCurr = transform.position;
        MoveTarget = transform.position;
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Velocity = new Vector3(Mathf.RoundToInt(Velocity.x), 
                               Mathf.RoundToInt(Velocity.y), 
                               Mathf.RoundToInt(Velocity.z));
        Velocity.x *= RowSize * 2;
        Velocity.y *= ColumnSize * 2;
        //CollisionHandling(ref Velocity);
        Move();

    }

    private void LateUpdate()
    {
        if (Moved && DelayMoveCoroutine == null)
        {
            CanMove = false;
            DelayMoveCoroutine = StartCoroutine(DelayMove());
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(moveCurr, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(MoveTarget, 0.2f);
    }

    private bool CollisionHandling(ref Vector3 velocity)
    {
        if (velocity.x == 0 && velocity.y == 0)
            return true;
        var hit = Physics2D.OverlapBox(transform.position + velocity, collider.bounds.size, 0);
        if (hit != null)
        {
            bool moveable = true;
            if (hit.gameObject.TryGetComponent<PushableComponent>(out var push))
            {
                if (!push.Push(velocity))
                {
                    moveable = false;
                }
            }
            else if (hit.gameObject.TryGetComponent<StopComponent>(out var stop))
                moveable = false;
            else if (hit.gameObject.TryGetComponent<Player>(out var player))
            {
                var tempVel = velocity;
                if (!player.GridMove.CollisionHandling(ref tempVel))
                {
                    moveable = false;
                }
            }
            if (!moveable)
            {
                Debug.DrawRay(transform.position, Vector3.right, Color.red, 0.2f);
                velocity = Vector3.zero;
                return false;
            }
            else
            {
                return true;
            }
        }
        return true;
    }

    private void Move()
    {
        if (Velocity.x != 0 && Velocity.y != 0)
        {
            Velocity.y = 0;
        }
        if (Vector3.Distance(moveCurr, MoveTarget) <= 0f)
        {
            MoveTarget = moveCurr + Velocity;
            Velocity = Vector3.zero;
        }
        moveCurr = Vector3.MoveTowards(moveCurr, MoveTarget, MoveSpeed * Time.deltaTime);
        rigidbody.MovePosition(moveCurr);
    }

    public bool TryMove(Vector3 velocity)
    {
        if (!CanMove)
            return false;
        if (velocity == Vector3.zero)
            return false;
        if (Vector3.Distance(moveCurr, MoveTarget) > 0f)
            return false;
        if (!CollisionHandling(ref velocity))
            return false;
        Moved = true;
        Velocity = velocity;
        //Move();
        return true;
    }

    IEnumerator DelayMove()
    {
        CanMove = false;
        yield return new WaitForSecondsRealtime(MoveDelay);
        Debug.Log(CanMove);
        DelayMoveCoroutine = null;
        CanMove = true;
        Moved = false;
    }
}
