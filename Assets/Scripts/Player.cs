using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GridMoveComponent))]
public class Player : MonoBehaviour
{
    private Animator animator;
    private new SpriteRenderer renderer;
    [HideInInspector] public GridMoveComponent GridMove;

    Vector2 moveDir = Vector2.zero;

    public static event Action PlayerMove;

    // Start is called before the first frame update
    void Start()
    {
        GridMove = GetComponent<GridMoveComponent>();
        if (TryGetComponent<Animator>(out var ani))
            animator = ani;
        if (TryGetComponent<SpriteRenderer>(out var ren))
            renderer = ren;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        float verticalInput = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);

        if (horizontalInput != 0 && verticalInput != 0)
        {
            horizontalInput = 0;
            verticalInput = 0;
        }

        Vector3 input = new Vector3(horizontalInput, verticalInput, 0);
        if (input != Vector3.zero)
        {
            PlayerMove?.Invoke();
        }
        GridMove.TryMove(input);
    }

    private void FixedUpdate()
    {
        
    }
}
