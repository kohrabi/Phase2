using UnityEngine;

[RequireComponent(typeof(GridMoveComponent))]
public class MoveableComponent : MonoBehaviour
{

    GridMoveComponent gridMove;

    // Start is called before the first frame update
    void Start()
    {
        gridMove = GetComponent<GridMoveComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = (Input.GetKeyDown(KeyCode.D) ? 1 : 0) - (Input.GetKeyDown(KeyCode.A) ? 1 : 0);
        float verticalInput = (Input.GetKeyDown(KeyCode.W) ? 1 : 0) - (Input.GetKeyDown(KeyCode.S) ? 1 : 0);
        gridMove.TryMove(new Vector3(horizontalInput, verticalInput, 0));
    }

}
