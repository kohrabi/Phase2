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
    public static Dictionary<string, List<Type> > CurrentRules = new Dictionary<string, List<Type> >(); 

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

        CheckRule(left, right, ref prevLeftAText, ref prevRightBText);
        CheckRule(up, down, ref prevUpAText, ref prevDownBText);
    }

    void CheckRule(Collider2D left, Collider2D right, ref INameText prevAText, ref INameText prevBText)
    {
        bool ran = false;
        if (left != null && right != null)
        {
            if (left.TryGetComponent<AText>(out var leftA) && right.TryGetComponent<BText>(out var rightB))
            {
                ran = true;
                var leftObj = GameObject.FindGameObjectWithTag(leftA.Text);
                //if (leftA.TryGetComponent(rightB.ComponentType, out var trash))
                if ((leftObj != null && !leftObj.TryGetComponent(rightB.ComponentType, out var trash)) ||
                    leftA != (AText)prevAText ||
                    prevBText.GetType() != rightB.GetType() ||
                    (prevBText.GetType() == rightB.GetType() && ((BText)prevBText).ComponentName != rightB.ComponentName))
                {
                    AddComponentToObjects(leftA.Text, rightB.ComponentType);
                    Debug.Log(leftA.Text + " is " + rightB.Text);
                    if (prevAText != null && prevBText != null)
                    {
                        if (prevRightBText.GetType() == typeof(BText))
                        {
                            RemoveComponentFromObjects(((AText)prevAText).Text, ((BText)prevBText).ComponentType);
                        }
                    }
                    prevAText = leftA;
                    prevBText = rightB;
                }
            }
            else if (left.TryGetComponent<AText>(out leftA) && right.TryGetComponent<AText>(out var rightA))
            {
                ran = true;
                if (leftA != (AText)prevAText ||
                    prevBText.GetType() != rightA.GetType() ||
                    (prevBText.GetType() == rightA.GetType() && ((AText)prevBText).Text != rightA.Text))
                {
                    ReplaceObjectsWithObject(leftA.Text, rightA.Text);
                    if (prevAText != null && prevBText != null)
                    {
                        if (prevBText.GetType() == typeof(BText))
                        {
                            RemoveComponentFromObjects(((AText)prevBText).Text, ((BText)prevBText).ComponentType);
                        }
                    }
                    prevAText = leftA;
                    prevBText = rightA;
                }
            }
        }
        if (!ran)
        {
            if (prevAText != null && prevBText != null)
            {
                if (prevBText.GetType() == typeof(BText))
                {
                    RemoveComponentFromObjects(((AText)prevAText).Text, ((BText)prevBText).ComponentType);
                }
                prevAText = null;
                prevBText = null;
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
        if (!CurrentRules.TryGetValue(tag, out var del))
            CurrentRules.Add(tag, new List<Type>());
        CurrentRules[tag].Add(componentType);
    }

    void ReplaceObjectsWithObject(string tagobjs, string tagobj)
    {
        var objs = GameObject.FindGameObjectsWithTag(tagobjs);
        var replaceObj = GameObject.FindGameObjectWithTag(tagobj);
        bool nullObj = false;
        if (replaceObj == null)
        {
            replaceObj = Resources.Load<GameObject>("Prefabs/Tile/" + tagobj + "/" + tagobj);
            nullObj = true;
        }
        foreach (var obj in objs)
        {
            var newObj = Instantiate(replaceObj, replaceObj.transform.parent);
            newObj.transform.position = obj.transform.position;
            if (nullObj)
            {
                foreach (var component in CurrentRules[tagobj])
                    newObj.AddComponent(component);
            }
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
        if (CurrentRules.TryGetValue(tag, out var rules))
        {
            rules.Remove(componentType);
        }
    }

   
}
