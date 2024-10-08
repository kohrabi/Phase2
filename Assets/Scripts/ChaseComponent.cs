using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridMoveComponent))]
public class ChaseComponent : MonoBehaviour
{
    private GridMoveComponent gridMove;
    [SerializeField] public Vector2 BoxSize = new Vector2(1f, 1f);
    [SerializeField] private new BoxCollider2D collider;

    private void Start()
    {
        gridMove = GetComponent<GridMoveComponent>();
        collider = GetComponent<BoxCollider2D>();
    }

    public Vector2 move;
    public void Chase()
    {
        if (!GridMoveComponent.CanMove)
            return;
        move = FindBestMove();
        TurnManager.Instance.OccupiedPos.Add(move);
        gridMove.TryMove(move);
    }

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

    private int count = 0;

    private Vector2 FindBestMove()
    {
        GameObject target = null;
        float minDis = Mathf.Infinity;
        foreach (var player in TurnManager.Instance.Players)
        {
            float dis = Vector3.Distance(player.transform.position, gridMove.MoveTarget);
            if (dis < minDis)
            {
                minDis = dis;
                target = player;
            }
        }

        if (target == null)
            return Vector2.zero;

        Vector2 start = gridMove.MoveTarget;
        Vector2 end = target.transform.position;

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        Node startNode = new Node(start);
        Node endNode = new Node(end);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            count++;
            Debug.Log(count);
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

        return Vector2.zero;
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

        return path.Count > 1 ? path[0].Position - startNode.Position : Vector2.zero;
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
        if (TurnManager.Instance.OccupiedPos.Contains(position))
            return false;

        List<Collider2D> colliders = detectGameObjects(position);
        int i = 0;
        for (; i < colliders.Count; i++)
        {
            if (colliders[i].gameObject.CompareTag("Baba"))
                return true;
            if (colliders[i].gameObject.CompareTag("RuleBox")) break;
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
