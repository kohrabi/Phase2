using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GridMoveComponent))]
public class MoveableComponent : MonoBehaviour
{
    [SerializeField] public float MoveDelay = 0.005f;
    GridMoveComponent gridMove;

    Vector2 moveDir = Vector2.zero;

    bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        gridMove = GetComponent<GridMoveComponent>();
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
            gridMove.TryMove(new Vector3(horizontalInput, verticalInput, 0));
            StartCoroutine(DelayMove());
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
