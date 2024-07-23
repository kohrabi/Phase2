using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class GridMove : MonoBehaviour
{
    [SerializeField] public float RowSize = 0.1f;
    [SerializeField] public float ColumnSize = 0.1f;
    [SerializeField] public float MoveSpeed = 0.32f;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Rigidbody2D rigidbody;

    Vector3 moveTarget = Vector3.zero; // Dung Xoa
    Vector3 moveCurrent = Vector3.zero; // Dung Xoa

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(sprite.bounds.size / 2);
        transform.position = new Vector3(sprite.bounds.size.x / 2, sprite.bounds.size.y / 2, 0);
        moveTarget = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = Vector3.zero;
        float horizontalInput = (Input.GetKeyDown(KeyCode.D) ? 1 : 0) - (Input.GetKeyDown(KeyCode.A) ? 1 : 0);
        float verticalInput = (Input.GetKeyDown(KeyCode.W) ? 1 : 0) - (Input.GetKeyDown(KeyCode.S) ? 1 : 0);

        moveTarget.x += horizontalInput * RowSize * 2;
        moveTarget.y += verticalInput * ColumnSize * 2;
        rigidbody.MovePosition(moveTarget);
    }
}
