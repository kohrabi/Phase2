
using System;
using UnityEngine;

public class GridMoveComponent : MonoBehaviour
{
    [SerializeField] public float RowSize = 0.1f;
    [SerializeField] public float ColumnSize = 0.1f;
    [SerializeField] public float MoveSpeed = 0.32f;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private BoxCollider2D collider;
    public Vector3 Velocity = Vector3.zero;

    Vector3 moveTarget = Vector3.zero; // Dung Xoa
    Vector3 movePoint = Vector3.zero; // Dung Xoa
    //RaycastHit2D[] castResults = new RaycastHit2D[7];


    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(
                                    Mathf.RoundToInt(transform.position.x / RowSize) * RowSize, 
                                    Mathf.RoundToInt(transform.position.y / ColumnSize) * ColumnSize, 
                                    0);
        moveTarget = transform.position;
        movePoint = transform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        Velocity = new Vector3(Mathf.RoundToInt(Velocity.x), 
                               Mathf.RoundToInt(Velocity.y), 
                               Mathf.RoundToInt(Velocity.z));
        Velocity.x *= RowSize * 2;
        Velocity.y *= ColumnSize * 2;

        if (Velocity.x != 0 && Velocity.y != 0)
        {
            Velocity.y = 0;
        }

        //CollisionHandling(ref Velocity);
        Move();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(moveTarget, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(movePoint, 0.2f);
    }

    private bool CollisionHandling(ref Vector3 velocity)
    {
        if (velocity.x == 0 && velocity.y == 0)
            return true;
        var hit = Physics2D.OverlapBox(transform.position + velocity, collider.size, 0);
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
            else
                moveable = false;
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
        /*
        int castCount = rigidbody.Cast(velocity.normalized, castResults, velocity.magnitude);
        if (castCount > 0)
        {
            bool moveable = true;
            for (int i = 0; i < MathF.Min(castCount, castResults.Length); i++)
            {
                var hit = castResults[i].rigidbody;
                if (hit == null) break;
                if (hit.gameObject.TryGetComponent<PushableComponent>(out var push))
                {
                    if (!push.Push(velocity))
                    {
                        moveable = false;
                    }
                }
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
        */
    }

    private void Move()
    {
        if (Vector3.Distance(moveTarget, movePoint) <= 0f)
        {
            movePoint = moveTarget + Velocity;
            Velocity = Vector3.zero;
        }
        moveTarget = Vector3.MoveTowards(moveTarget, movePoint, MoveSpeed * Time.deltaTime);
        rigidbody.MovePosition(moveTarget);
    }

    public bool TryMove(Vector3 velocity)
    {
        if (Vector3.Distance(moveTarget, movePoint) > 0f)
            return false;
        if (!CollisionHandling(ref velocity))
            return false;
        Velocity = velocity;
        //Move();
        return true;
    }
}
