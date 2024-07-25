using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PushableComponent))]
public class AText : MonoBehaviour
{
    [SerializeField] public float RowSize = 0.5f;
    [SerializeField] public float ColumnSize = 0.5f;
    public string text;

}
