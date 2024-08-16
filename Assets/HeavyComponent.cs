using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(GridMoveComponent))]
public class HeavyComponent : MonoBehaviour
{
    const float undoingDelay = 0.3f;
    GridMoveComponent gridMove;
    float undoingTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        gridMove = GetComponent<GridMoveComponent>();   
    }

    void Update()
    {
        if (undoingTimer > 0)
            undoingTimer -= Time.deltaTime;
        if (!gridMove.undoing)
        {
            if (undoingTimer <= 0)
                gridMove.TryMove(Vector3.down);
        }
        else
            undoingTimer = undoingDelay;
        
    }
}
