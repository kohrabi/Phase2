using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridMoveComponent))]
public class PushableComponent : MonoBehaviour
{
    GridMoveComponent gridMove;

    // Start is called before the first frame update
    void Start()
    {
        gridMove = GetComponent<GridMoveComponent>();    
    }


    public bool Push(Vector3 velocity)
    {
        return gridMove.TryMove(velocity);
    }
}
