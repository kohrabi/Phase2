using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class GridMove : MonoBehaviour
{
    [SerializeField] public float RowSize = 0.1f;
    [SerializeField] public float ColumnSize = 0.1f;
    [SerializeField] public float MoveSpeed = 0.32f;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Rigidbody2D MyRigidbody;



    [SerializeField] private Vector2 Destination;
    [SerializeField] private float MoveDistance = 0.5f;

    void Start()
    {
        Destination = transform.position;
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, Destination, MoveSpeed * Time.deltaTime);
        if (Vector2.Distance(Destination, transform.position) <= 0.05f)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
                Destination += new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
                Destination += new Vector2(0f, Input.GetAxisRaw("Vertical"));
        }
    }
}