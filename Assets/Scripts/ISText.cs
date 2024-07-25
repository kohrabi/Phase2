using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[RequireComponent(typeof(PushableComponent), typeof(GridMoveComponent))]
public class ISText : MonoBehaviour
{
    [SerializeField] public float RowSize = 0.5f;
    [SerializeField] public float ColumnSize = 0.5f;
    [SerializeField] private new BoxCollider2D collider;

    GridMoveComponent gridMove;
    AText prevAText;
    BText prevBText;
    // Start is called before the first frame update
    void Start()
    {
        gridMove = GetComponent<GridMoveComponent>();   
    }

    private void FixedUpdate()
    {
        Check();
    }

    public void Check()
    {
        Collider2D left = 
            Physics2D.OverlapBox(gridMove.MoveTarget - new Vector3(ColumnSize * 2, 0), collider.bounds.size, 0, LayerMask.GetMask("RuleBox"));
        Collider2D right = 
            Physics2D.OverlapBox(gridMove.MoveTarget + new Vector3(ColumnSize * 2, 0), collider.bounds.size, 0, LayerMask.GetMask("RuleBox"));
        Collider2D up = 
            Physics2D.OverlapBox(gridMove.MoveTarget + new Vector3(0, RowSize * 2), collider.bounds.size, 0, LayerMask.GetMask("RuleBox"));
        Collider2D down = 
            Physics2D.OverlapBox(gridMove.MoveTarget - new Vector3(0, RowSize * 2), collider.bounds.size, 0, LayerMask.GetMask("RuleBox"));

        Vector2 dir = Vector2.zero;
        if (left != null && right != null)
        {
            if (left.TryGetComponent<AText>(out var leftA) && right.TryGetComponent<BText>(out var rightB))
            {
                dir = Vector2.right;
                if (leftA != prevAText || rightB != prevBText)
                {
                    AddComponentToObjects(leftA.text, rightB.ComponentType);
                    Debug.Log(leftA.text + "is" + rightB.Text);
                    if (prevAText != null && prevBText != null) RemoveComponentFromObjects(prevAText.text, prevBText.ComponentType);
                    prevAText = leftA;
                    prevBText = rightB;
                }
            }
        }
        if (up != null && down != null)
        {
            if (up.TryGetComponent<AText>(out var upA) && down.TryGetComponent<BText>(out var downB))
            {
                dir = Vector2.down;
                if (upA != prevAText || downB != prevBText)
                {
                    AddComponentToObjects(upA.text, downB.ComponentType);
                    Debug.Log(upA.text + "is" + downB.Text);
                    if (prevAText != null && prevBText != null) RemoveComponentFromObjects(prevAText.text, prevBText.ComponentType);
                    prevAText = upA;
                    prevBText = downB;
                }
            }
        }   
        // remove old rule
        if (dir == Vector2.zero && prevAText != null && prevBText != null)
        {
            RemoveComponentFromObjects(prevAText.text, prevBText.ComponentType);
            prevAText = null;
            prevBText = null;
        }

    }

    void AddComponentToObjects(string tag, Type componentType)
    {
        var objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (var obj in objects)
        {
            obj.AddComponent(componentType);
        }
    }

    void RemoveComponentFromObjects(string tag, Type componentType)
    {
        var objects = GameObject.FindGameObjectsWithTag(prevAText.text);
        foreach (var obj in objects)
        {
            var component = obj.gameObject.GetComponent(prevBText.ComponentType);
            Destroy(component);
        }
    }

   
}
