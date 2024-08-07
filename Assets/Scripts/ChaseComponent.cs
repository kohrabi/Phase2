using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ChaseComponent : MonoBehaviour
{
    private GridMoveComponent gridMove;
    [SerializeField] public Vector2 BoxSize = new Vector2(1f, 1f);
    [SerializeField] private new BoxCollider2D collider;

    [SerializeField] private GameObject player;

    private void Start()
    {
        gridMove = GetComponent<GridMoveComponent>();
        collider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        Chase();
    }

    public Vector2 move;
    public void Chase()
    {
        move = FindBestMove();
        gridMove.TryMove(move);
    }

    //private Vector2 FindBestMove()//Tra ve 1 huong
    //{
    //    Vector2 BestMove = Vector2.zero;
    //    float minDistance = Mathf.Infinity;
    //    List<Collider2D> left = detectGameObjects(gridMove.MoveTarget + new Vector3(-BoxSize.x, 0));
    //    if (CheckMoveable(left))
    //    {
    //        float dis = Vector2.Distance(gridMove.MoveTarget + new Vector3(-BoxSize.x, 0), player.transform.position);
    //        if (dis < minDistance)
    //        {
    //            minDistance = dis;
    //            BestMove = Vector2.left;
    //        }
    //    }
    //    List<Collider2D> right = detectGameObjects(gridMove.MoveTarget + new Vector3(BoxSize.x, 0));
    //    if (CheckMoveable(right))
    //    {
    //        float dis = Vector2.Distance(gridMove.MoveTarget + new Vector3(BoxSize.x, 0), player.transform.position);
    //        if (dis < minDistance)
    //        {
    //            minDistance = dis;
    //            BestMove = Vector2.right;
    //        }
    //    }
    //    List<Collider2D> up = detectGameObjects(gridMove.MoveTarget + new Vector3(0, BoxSize.y));
    //    if (CheckMoveable(up))
    //    {
    //        float dis = Vector2.Distance(gridMove.MoveTarget + new Vector3(0, BoxSize.y), player.transform.position);
    //        if (dis < minDistance)
    //        {
    //            minDistance = dis;
    //            BestMove = Vector2.up;
    //        }
    //    }
    //    List<Collider2D> down = detectGameObjects(gridMove.MoveTarget + new Vector3(0, -BoxSize.y));
    //    if (CheckMoveable(down))
    //    {
    //        float dis = Vector2.Distance(gridMove.MoveTarget + new Vector3(0, -BoxSize.y), player.transform.position);
    //        if (dis < minDistance)
    //        {
    //            minDistance = dis;
    //            BestMove = Vector2.down;
    //        }
    //    }
    //    return BestMove;
    //}

    public class Node
    {
        public Vector2 Position;
        public float GCost; // Cost from start to current node
        public float HCost; // Heuristic cost to the target
        public float FCost => GCost + HCost; // Total cost
        public Node Parent;

        public Node(Vector2 position)
        {
            Position = position;
        }
    }

    private Vector2 FindBestMove()
    {
        Vector2 start = gridMove.MoveTarget;
        Vector2 end = player.transform.position;

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        Node startNode = new Node(start);
        Node endNode = new Node(end);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost ||
                    openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost)
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.Position == endNode.Position)
            {
                return RetracePath(startNode, currentNode);
            }

            foreach (Vector2 neighborPos in GetNeighbors(currentNode.Position))
            {
                if (!IsMoveable(neighborPos) || closedList.Contains(new Node(neighborPos)))
                {
                    continue;
                }

                Node neighborNode = new Node(neighborPos);
                float newCostToNeighbor =
                    currentNode.GCost + Vector2.Distance(currentNode.Position, neighborNode.Position);
                if (newCostToNeighbor < neighborNode.GCost || !openList.Contains(neighborNode))
                {
                    neighborNode.GCost = newCostToNeighbor;
                    neighborNode.HCost = Vector2.Distance(neighborNode.Position, endNode.Position);
                    neighborNode.Parent = currentNode;

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        return Vector2.zero; // Return zero if no path is found
    }

    private Vector2 RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();

        return path.Count > 1 ? path[1].Position - startNode.Position : Vector2.zero;
    }

    private IEnumerable<Vector2> GetNeighbors(Vector2 position)
    {
        yield return position + Vector2.left;
        yield return position + Vector2.right;
        yield return position + Vector2.up;
        yield return position + Vector2.down;
    }

    private bool IsMoveable(Vector2 position)
    {
        List<Collider2D> colliders = detectGameObjects(position);
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
