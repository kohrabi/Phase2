using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseObject : MonoBehaviour
{
    private GridMoveComponent gridMove;
    [SerializeField] public Vector2 BoxSize;
    [SerializeField] private new BoxCollider2D collider;

    [SerializeField] private GameObject player;

    private void Start()
    {
        gridMove = GetComponent<GridMoveComponent>();
        collider = GetComponent<BoxCollider2D>();
    }

    //private void Update()
    //{
    //    Chase();
    //}

    public Vector2 move;
    public void Chase()
    {
        move = FindBestMove();
        gridMove.TryMove(move);
    }

    private Vector2 FindBestMove()//Tra ve 1 huong
    {
        Vector2 BestMove = Vector2.zero;
        float minDistance = Mathf.Infinity;
        List<Collider2D> left = detectGameObjects(gridMove.MoveTarget + new Vector3(-BoxSize.x, 0));
        if (CheckMoveable(left))
        {
            float dis = Vector2.Distance(gridMove.MoveTarget + new Vector3(-BoxSize.x, 0), player.transform.position);
            if (dis < minDistance)
            {
                minDistance = dis;
                BestMove = Vector2.left;
            }
        }
        List<Collider2D> right = detectGameObjects(gridMove.MoveTarget + new Vector3(BoxSize.x, 0));
        if (CheckMoveable(right))
        {
            float dis = Vector2.Distance(gridMove.MoveTarget + new Vector3(BoxSize.x, 0), player.transform.position);
            if (dis < minDistance)
            {
                minDistance = dis;
                BestMove = Vector2.right;
            }
        }
        List<Collider2D> up = detectGameObjects(gridMove.MoveTarget + new Vector3(0, BoxSize.y));
        if (CheckMoveable(up))
        {
            float dis = Vector2.Distance(gridMove.MoveTarget + new Vector3(0, BoxSize.y), player.transform.position);
            if (dis < minDistance)
            {
                minDistance = dis;
                BestMove = Vector2.up;
            }
        }
        List<Collider2D> down = detectGameObjects(gridMove.MoveTarget + new Vector3(0, -BoxSize.y));
        if (CheckMoveable(down))
        {
            float dis = Vector2.Distance(gridMove.MoveTarget + new Vector3(0, -BoxSize.y), player.transform.position);
            if (dis < minDistance)
            {
                minDistance = dis;
                BestMove = Vector2.down;
            }
        }
        return BestMove;
    }

    private bool CheckMoveable(List<Collider2D> colliders)
    {
        int i = 0;
        for (; i < colliders.Count; i++)
        {
            if (colliders[i].gameObject.CompareTag("RuleBox")) break;
            if (colliders[i].gameObject.CompareTag("Wall")) break;
            if (!colliders[i].gameObject.TryGetComponent<PushableComponent>(out var pushable)) break;
        }

        return i >= colliders.Count;
    }

    private List<Collider2D> detectGameObjects(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, BoxSize, 0);
        return new List<Collider2D>(colliders);
    }

}
