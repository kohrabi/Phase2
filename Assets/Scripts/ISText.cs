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
    INameText prevLeftAText;
    INameText prevRightBText;
    INameText prevUpAText;
    INameText prevDownBText;
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
                if (leftA != (AText)prevLeftAText || (prevRightBText.GetType() == rightB.GetType() && (BText)prevRightBText != rightB))
                {
                    AddComponentToObjects(leftA.Text, rightB.ComponentType);
                    Debug.Log(leftA.Text + " is " + rightB.Text);
                    if (prevLeftAText != null && prevRightBText != null)
                    {
                        if (prevRightBText.GetType() == typeof(BText))
                            RemoveComponentFromObjects(((AText)prevLeftAText).Text, ((BText)prevRightBText).ComponentType);
                    }
                    prevLeftAText = leftA;
                    prevRightBText = rightB;
                }
            }
            if (left.TryGetComponent<AText>(out leftA) && right.TryGetComponent<AText>(out var rightA))
            {
                dir = Vector2.right;
                if (leftA != (AText)prevLeftAText || (prevRightBText.GetType() == rightA.GetType() && (AText)prevRightBText != rightA))
                {
                    ReplaceObjectsWithObject(leftA.Text, rightA.Text);
                    Debug.Log(leftA.Text + "is" + rightA.Text);
                    if (prevLeftAText != null && prevRightBText != null)
                    {
                        if (prevRightBText.GetType() == typeof(BText))
                            RemoveComponentFromObjects(((AText)prevLeftAText).Text, ((BText)prevRightBText).ComponentType);
                    }
                    prevLeftAText = leftA;
                    prevRightBText = rightA;
                }
            }
        }
        if (up != null && down != null)
        {
            if (up.TryGetComponent<AText>(out var upA) && down.TryGetComponent<BText>(out var downB))
            {
                dir = Vector2.down;
                if (upA != (AText)prevUpAText || (prevDownBText.GetType() == downB.GetType() && (BText)prevDownBText != downB))
                {
                    AddComponentToObjects(upA.Text, downB.ComponentType);
                    Debug.Log(upA.Text + " is " + downB.Text);
                    if (prevUpAText != null && prevDownBText != null)
                    {
                        if (prevDownBText.GetType() == typeof(BText))
                            RemoveComponentFromObjects(((AText)prevUpAText).Text, ((BText)prevDownBText).ComponentType);
                    }
                    prevUpAText = upA;
                    prevDownBText = downB;
                }
            }
            if (left.TryGetComponent<AText>(out upA) && right.TryGetComponent<AText>(out var downA))
            {
                dir = Vector2.right;
                if (upA != (AText)prevUpAText || (prevDownBText.GetType() == downA.GetType() && (AText)prevDownBText != downA))
                {
                    ReplaceObjectsWithObject(upA.Text, downA.Text);
                    Debug.Log(upA.Text + " is " + downA.Text);
                    if (prevUpAText != null && prevDownBText != null)
                    {
                        if (prevDownBText.GetType() == typeof(BText))
                            RemoveComponentFromObjects(((AText)prevUpAText).Text, ((BText)prevDownBText).ComponentType);
                    }
                    prevUpAText = upA;
                    prevDownBText = downA;
                }
            }
        }   
        // remove old rule
        if (dir == Vector2.zero)
        {
            if (prevLeftAText != null && prevRightBText != null)
            {
                if (prevRightBText.GetType() == typeof(BText))
                    RemoveComponentFromObjects(((AText)prevLeftAText).Text, ((BText)prevRightBText).ComponentType);
                prevLeftAText = null;
                prevRightBText = null;
            }
            if (prevUpAText != null && prevDownBText != null)
            {
                if (prevDownBText.GetType() == typeof(BText))
                    RemoveComponentFromObjects(((AText)prevUpAText).Text, ((BText)prevDownBText).ComponentType);
                prevUpAText = null;
                prevDownBText = null;
            }
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

    void ReplaceObjectsWithObject(string tagobjs, string tagobj)
    {
        var objs = GameObject.FindGameObjectsWithTag(tagobjs);
        var replaceObj = GameObject.FindGameObjectWithTag(tagobj);
        foreach (var obj in objs)
        {
            var newObj = Instantiate(replaceObj, replaceObj.transform.parent);
            newObj.transform.position = obj.transform.position;
            if (newObj.TryGetComponent<GridMoveComponent>(out var grid))
            {
                grid.SetPosition(obj.transform.position);
            }
            obj.SetActive(false);
            UndoManager.Instance.AddToCurrentUndo(newObj, Vector3.zero, true, obj);
        }
    }

    void RemoveComponentFromObjects(string tag, Type componentType)
    {
        var objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (var obj in objects)
        {
            var component = obj.gameObject.GetComponent(componentType);
            Destroy(component);
        }
    }

   
}
