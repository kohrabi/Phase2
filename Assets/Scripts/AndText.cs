using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[RequireComponent(typeof(PushableComponent), typeof(GridMoveComponent))]
public class AndText : MonoBehaviour
{

    [SerializeField] public float RowSize = 0.5f;
    [SerializeField] public float ColumnSize = 0.5f;
    [SerializeField] private new BoxCollider2D collider;
    public static Dictionary<string, List<string>> Tags = new Dictionary<string, List<string>>();
    string prevUp, prevLeft;
    Collider2D prevRight;
    Collider2D prevDown;

    GridMoveComponent gridMove;
    // Start is called before the first frame update
    void Start()
    {


        gridMove = GetComponent<GridMoveComponent>();


    }

    // Update is called once per frame
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
        CheckRule(up, down,ref prevUp,ref prevDown,0);
        CheckRule(left,right,ref prevLeft,ref prevRight,1);
    }
    void CheckRule(Collider2D up, Collider2D down, ref string prevUp, ref Collider2D prevDown,int i)
    {
        if (up != null && down != null)
        {
            if (!(up.GetComponent<AText>() == null && up.GetComponent<BText>() == null) && !(down.GetComponent<AText>() == null && down.GetComponent<BText>() == null))
            {
                Debug.Log("And");
                string prev;
                if (up.TryGetComponent<AText>(out var Up))
                {
                    if (i == 0) prev = Up.UpText; else prev = Up.LeftText;
                }
                else  
                {
                    if (i == 0) prev = up.GetComponent<BText>().UpText; 
                    else prev= up.GetComponent<BText>().LeftText;
                }
                if (prev != null)
                {
                    if (down.TryGetComponent<AText>(out var Down))
                    {
                        ISText.ReplaceObjectsWithObject(prev, Down.Text);
                        if(i==0) Down.UpText = prev; else Down.LeftText = prev;
                    }
                    else
                    {
                        ISText.AddComponentToObjects(prev, down.GetComponent<BText>().ComponentType);
                        if(i==0) down.GetComponent<BText>().UpText = prev; else down.GetComponent<BText>().LeftText = prev;
                    }
                    if (prevDown != null && prevDown.TryGetComponent<BText>(out var DownB))
                    {
                        ISText.RemoveComponentFromObjects(prevUp, DownB.ComponentType);
                        if (down != prevDown) {if(i==0) DownB.UpText = null; else DownB.LeftText = null; }
                    }
                    prevDown = down;
                    prevUp = prev;

                }
                else
                {
                    if (prevDown != null && prevDown.TryGetComponent<BText>(out var DownB))
                    {
                        ISText.RemoveComponentFromObjects(prevUp, DownB.ComponentType);
                        if (down != prevDown) { if (i == 0) DownB.UpText = null; else DownB.LeftText = null; }
                    }
                    prevDown = null;
                    prevUp = null;
                }
            }
        }
    }
}
