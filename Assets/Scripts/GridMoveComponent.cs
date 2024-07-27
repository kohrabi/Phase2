
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))]
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
    private Animator animator;
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
        animator = GetComponent<Animator>();
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
            UndoManager.Instance.NextTurn();
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
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position + velocity, collider.bounds.size, 0);
        if (hits != null && hits.Length > 0)
        {
            bool moveable = true;
            for (int i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                if (hit == null)
                    break;
                if (hit.gameObject.TryGetComponent<PushableComponent>(out var push))
                {
                    if (!push.Push(velocity))
                    {
                        moveable = false;
                        break;
                    }
                }
                else if (hit.gameObject.TryGetComponent<StopComponent>(out var stop))
                {
                    moveable = false;
                    break;
                }
                //else if (hit.gameObject.TryGetComponent<Player>(out var player))
                //{
                //    var tempVel = velocity;
                //    if (!player.GridMove.CollisionHandling(ref tempVel))
                //    {
                //        moveable = false;
                //        break;
                //    }
                //}
            }
            if (!moveable)
            {
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
        if (Velocity != velocity)
            UndoManager.Instance.AddToCurrentUndo(gameObject, transform.position);
        Velocity = velocity;
        // Animate
        HandleAnimation(velocity);
        //Move();
        return true;
    }

    void HandleAnimation(Vector3 velocity, bool inverted = false)
    {
        if (animator != null)
        {
            animator.logWarnings = false;
            velocity.x = Math.Sign(velocity.x);
            velocity.y = Math.Sign(velocity.y);
            animator.SetFloat("WalkFrame", ((animator.GetFloat("WalkFrame") * 3 + 1)) % 4 / 3);
            if (velocity.y == 1)
            {
                // Up
                animator.SetFloat("WalkDir", inverted ? 0.25f : 0f);
            }
            else if (velocity.y == -1)
            {
                // Down
                animator.SetFloat("WalkDir", inverted ? 0f : 0.25f);
            }
            else if (velocity.x == -1)
            {
                // Left
                animator.SetFloat("WalkDir", inverted ? 1f : 0.75f);
            }
            else if (velocity.x == 1)
            {
                // Right
                animator.SetFloat("WalkDir", inverted ? 0.75f : 1f);
            }
            animator.logWarnings = true;
        }
    }

    public void UndoMove(Vector3 velocity)
    {
        Velocity = velocity;
        HandleAnimation(velocity, true);
    }

    IEnumerator DelayMove()
    {
        CanMove = false;
        yield return new WaitForSecondsRealtime(MoveDelay);
        DelayMoveCoroutine = null;
        CanMove = true;
        Moved = false;
    }
}
