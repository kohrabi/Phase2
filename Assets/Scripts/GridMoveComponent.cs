
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))]
public class GridMoveComponent : MonoBehaviour
{
    public const float RowSize = 0.5f;
    public const float ColumnSize = 0.5f;
    public const float MoveSpeed = 10f;
    public const float MoveDelay = 0.2f;

    public static bool Moved = false;
    public static bool CanMove = true;
    public static event EventHandler TurnOnPlayerInput;
    public static event EventHandler TurnEnded;
    public static Coroutine DelayMoveCoroutine = null;
    
    private new Rigidbody2D rigidbody;
    private Animator animator;
    private new BoxCollider2D collider;
    public Vector3 Velocity = Vector3.zero;
    [HideInInspector] public bool undoing = false;

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
            TurnOnPlayerInput?.Invoke(this, null);
            // Waiting till next turn
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

    private int CollisionHandling(ref Vector3 velocity)
    {
        if (velocity.x == 0 && velocity.y == 0)
            return 1;
        //Collider2D[] hits = Physics2D.BoxCastAll(transform.position, collider.bounds.size, 0, velocity.normalized, velocity.magnitude);
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position + velocity, collider.bounds.size, 0);
        if (hits != null && hits.Length > 0)
        {
            int moveable = 1;
            for (int i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                if (hit == null)
                    break;
                if (hit.gameObject.TryGetComponent<PushableComponent>(out var push))
                {
                    if (!push.Push(velocity))
                    {
                        moveable = 0;
                        /*
                        if ((hit.rigidbody.position - (Vector2)transform.position).magnitude < velocity.magnitude &&
                        (hit.rigidbody.position - (Vector2)transform.position).magnitude > 1)
                        {
                            moveable = 2;
                            velocity = hit.rigidbody.position - (Vector2)transform.position;
                            if (velocity.y != 0)
                                velocity.y += -Math.Sign(velocity.y) * hit.collider.bounds.size.y;
                            else
                                velocity.x += -Math.Sign(velocity.x) * hit.collider.bounds.size.x;
                            if (velocity == Vector3.zero)
                                moveable = 0;
                        }
                        */
                        break;
                    }
                }
                else if (hit.gameObject.TryGetComponent<StopComponent>(out var stop))
                {
                    moveable = 0;
                    /*
                    if ((hit.rigidbody.position - (Vector2)transform.position).magnitude < velocity.magnitude &&
                        (hit.rigidbody.position - (Vector2)transform.position).magnitude > 1)
                    {
                        moveable = 2;
                        velocity = hit.rigidbody.position - (Vector2)transform.position;
                        if (velocity.y != 0)
                            velocity.y += -Math.Sign(velocity.y) * hit.collider.bounds.size.y;
                        else 
                            velocity.x += -Math.Sign(velocity.x) * hit.collider.bounds.size.x;
                    }
                    */
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
            if (moveable == 0)
            {
                velocity = Vector3.zero;
                return 0;
            }
            else
            {
                return 1;
            }
        }
        return 1;
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
        //moveCurr = Vector3.Lerp(moveCurr, MoveTarget, 0.5f);
        rigidbody.MovePosition(moveCurr);
        if (moveCurr == MoveTarget)
        {
            undoing = false;
        }
    }

    public bool TryMove(Vector3 velocity)
    {
        if (!CanMove)
            return false;
        if (velocity == Vector3.zero)
            return false;
        if (Vector3.Distance(moveCurr, MoveTarget) > 0f)
            return false;
        if (CollisionHandling(ref velocity) == 0)
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
        Moved = true;
        undoing = true;
        HandleAnimation(velocity, true);
    }

    public void SetPosition(Vector3 position)
    {
        moveCurr = position;
        MoveTarget = position;
        transform.position = position;
    }
    IEnumerator DelayMove()
    {
        CanMove = false;
        yield return new WaitForSecondsRealtime(MoveDelay);
        if (!undoing)
            UndoManager.Instance.NextTurn();
        TurnEnded?.Invoke(this, null);
        undoing = false;
        DelayMoveCoroutine = null;
        CanMove = true;
        Moved = false;
    }
}
