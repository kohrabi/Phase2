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
            if (up.TryGetComponent<AText>(out var upA) && down.TryGetComponent<AText>(out var downA))
            {
                if (upA.UpText != null) upA.UpText = upA;
                {
                    Debug.Log(upA.Text + " and " + downA.Text);
                    if (prevUpA != null && prevDownA != null && prevUpA.Text != upA.Text && prevDownA.Text != downA.Text)
                    {
                        Tags[prevUpA.Text] = new List<string> { ((AText)prevUpA).Text };
                        Tags[prevDownA.Text] = new List<string> { ((AText)prevDownA).Text };
                    }
                    foreach (var s in Tags[upA.Text]) if (!Tags[downA.Text].Contains(s)) Tags[downA.Text].Add(s);
                    foreach (var s in Tags[downA.Text]) if (Tags[upA.Text].Contains(s)) Tags[upA.Text].Add(s);
                    downA.UpText = upA.UpText;
                   // prevUpA.UpText = null;
                   // prevDownA.UpText = null;
                    prevUpA = upA;
                    prevDownA = downA;
                   prevUpB = null;
                    prevDownB = null;
                }
            }

            else if ( down.TryGetComponent<BText>(out var downB))
            {
                if (upA.UpText != null)
                {
                    downB.UpText = upA.UpText;
                    Debug.Log(upA.Text + " and " + downB.Text);
                    if (prevDownB != null)
                    {
                        ISText.RemoveComponentFromObjects(prevDownB.UpText.Text, prevDownB.ComponentType);
                        prevDownB.UpText = prevUpA.UpText = null;
                    }
                    prevUpB = null;
                    prevDownA = null;
                    prevUpA = upA;
                    prevDownB = downB;
                    ISText.ReplaceObjectsWithObject(upA.UpText.Text, upA.Text);
                    ISText.AddComponentToObjects(upA.UpText.Text, downB.ComponentType);
                }
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
