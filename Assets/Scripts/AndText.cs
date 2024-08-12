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

    AText prevUpA, prevDownA, prevLeftA, prevRightA;
    BText prevUpB, prevDownB, prevLeftB, prevRightB;

    GridMoveComponent gridMove;
    // Start is called before the first frame update
    void Start()
    {
        List<string> list = new List<string>{"Baba","Flag","Rock","Wall" };
        foreach (string s in list)
        {
            if(!Tags.TryGetValue(s, out var l))
            {
                Tags.Add(s, new List<string>());
                Tags[s].Add(s);
            }
        }

        gridMove = GetComponent<GridMoveComponent>();


        prevDownA = prevLeftA = prevRightA = prevUpA = null;
        prevDownB = prevLeftB = prevRightB = prevUpB = null;
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




        if (up != null && down != null)
        {
            if (up.TryGetComponent<AText>(out var upA))
            {
                if(upA != null) { }
            }

           

        }
    }
    public static List<GameObject> FindTag(string tag)
    {
        List<GameObject> result = new List<GameObject>();
        foreach (var s in Tags[tag])
        {
            result.AddRange(GameObject.FindGameObjectsWithTag(s));
        }
        return result;
    }
}
