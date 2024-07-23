using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class GridMove : MonoBehaviour
{
    [SerializeField] public float RowSize = 0.1f;
    [SerializeField] public float ColumnSize = 0.1f;
    [SerializeField] public float MoveSpeed = 0.32f;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Rigidbody2D rigidbody;

    Vector3 moveTarget = Vector3.zero; // Dung Xoa
    Vector3 movePoint = Vector3.zero; // Dung Xoa
    RaycastHit2D[] rayHits = new RaycastHit2D[7];

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(sprite.bounds.size / 2);
        transform.position = new Vector3(sprite.bounds.size.x / 2, sprite.bounds.size.y / 2, 0);
        moveTarget = transform.position;
        movePoint = transform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = Vector3.zero;
        float horizontalInput = (Input.GetKeyDown(KeyCode.D) ? 1 : 0) - (Input.GetKeyDown(KeyCode.A) ? 1 : 0);
        float verticalInput = (Input.GetKeyDown(KeyCode.W) ? 1 : 0) - (Input.GetKeyDown(KeyCode.S) ? 1 : 0);

        velocity.x += horizontalInput * RowSize * 2;
        velocity.y += verticalInput * ColumnSize * 2;

        CollisionHandling(ref velocity);
        if (Vector3.Distance(moveTarget, movePoint) <= 0f)
        {
            if (Mathf.Abs(Mathf.Sign(velocity.x)) == 1 || Mathf.Abs(Mathf.Sign(velocity.y)) == 1)
            {
                movePoint = moveTarget + velocity;
            }
        }
        moveTarget = Vector3.MoveTowards(moveTarget, movePoint, MoveSpeed * Time.deltaTime);
        rigidbody.MovePosition(moveTarget);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(moveTarget, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(movePoint, 0.2f);
    }

    private void CollisionHandling(ref Vector3 velocity)
    {
        if (rigidbody.Cast(velocity, rayHits, velocity.magnitude) > 0)
        {
            velocity = Vector3.zero;
        }
    }
}
