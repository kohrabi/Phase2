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
    public Vector2 PlayerInput;
    public bool isMoving => Destination != (Vector2)transform.position;

    void Start()
    {
        Destination = transform.position;
    }

    void Update()
    {
        PlayerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Movement();
    }


    private void Movement()
    {
        transform.position = Vector2.MoveTowards(transform.position, Destination, MoveSpeed * Time.deltaTime);
        if (isMoving)
            return;
        if (Mathf.Abs(PlayerInput.x) == 1f)
            Destination += new Vector2(PlayerInput.x * MoveDistance, 0f);
        else if (Mathf.Abs(PlayerInput.y) == 1f)
            Destination += new Vector2(0f, PlayerInput.y * MoveDistance);
    }
}