using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GridMoveComponent))]
public class Player : MonoBehaviour
{
    [SerializeField] public float MoveDelay = 0.2f;
    private Animator animator;
    private SpriteRenderer renderer;
    GridMoveComponent gridMove;

    Vector2 moveDir = Vector2.zero;

    bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        gridMove = GetComponent<GridMoveComponent>();
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
        if (Input.GetKeyUp(KeyCode.D) ||
            Input.GetKeyUp(KeyCode.A) ||
            Input.GetKeyUp(KeyCode.W) ||
            Input.GetKeyUp(KeyCode.S))
            canMove = true;

        if (canMove)
        {
            if (horizontalInput != 0 && verticalInput != 0)
            {
                horizontalInput = 0;
                verticalInput = 0;
            }
            StartCoroutine(DelayMove());
            if (gridMove.TryMove(new Vector3(horizontalInput, verticalInput, 0)))
            {

                if (horizontalInput != 0 || verticalInput != 0)
                {
                    if (animator != null)
                    {
                        animator.SetFloat("WalkFrame", ((animator.GetFloat("WalkFrame") * 3 + 1)) % 4 / 3);
                        if (verticalInput == 1)
                        {
                            animator.SetFloat("WalkDir", 0.5f);
                        }
                        else if (verticalInput == -1)
                        {
                            animator.SetFloat("WalkDir", 0f);
                        }
                        else
                        {
                            animator.SetFloat("WalkDir", 1f);
                            renderer.flipX = horizontalInput < 0;
                        }
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        
    }

    IEnumerator DelayMove()
    {
        canMove = false;
        yield return new WaitForSecondsRealtime(MoveDelay);
        canMove = true;
    }

}
