using System.Collections;
using System.Collections.Generic;
using System.Numerics;
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

    Vector3 moveTarget = Vector3.zero; // Dung Xoa
    Vector3 moveCurrent = Vector3.zero; // Dung Xoa

    private Vector2 Direction;


    //void Start()
    //{
    //    Debug.Log(sprite.bounds.size / 2);
    //    transform.position = new Vector3(sprite.bounds.size.x / 2, sprite.bounds.size.y / 2, 0);
    //    moveTarget = transform.position;
    //}

    private bool isMoving = false;

    void Update()
    {
        //Vector3 velocity = Vector3.zero;
        //float horizontalInput = (Input.GetKeyDown(KeyCode.D) ? 1 : 0) - (Input.GetKeyDown(KeyCode.A) ? 1 : 0);
        //float verticalInput = (Input.GetKeyDown(KeyCode.W) ? 1 : 0) - (Input.GetKeyDown(KeyCode.S) ? 1 : 0);

        //moveTarget.x += horizontalInput * RowSize * 2;
        //moveTarget.y += verticalInput * ColumnSize * 2;
        //rigidbody.MovePosition(moveTarget);

        if (isMoving == false)
        {
            if (Input.GetKeyDown(KeyCode.W))
                StartCoroutine(Move(Vector2.up));
            else if (Input.GetKeyDown(KeyCode.S))
                StartCoroutine(Move(Vector2.down));
            else if (Input.GetKeyDown(KeyCode.D))
                StartCoroutine(Move(Vector2.right));
            else if (Input.GetKeyDown(KeyCode.A))
                StartCoroutine(Move(Vector2.left));
        }
    }

    [SerializeField] private float WaitTime = 0.1f;

    private IEnumerator Move(Vector2 direction)
    {
        isMoving = true;
        MyRigidbody.MovePosition(MyRigidbody.position + MoveSpeed * direction);
        yield return new WaitForSeconds(WaitTime);
        isMoving = false;
    }
}
