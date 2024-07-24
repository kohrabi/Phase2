using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ISText : MonoBehaviour
{
    public float distance;
    public Transform Left, Right,Up,Down;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        Check();
    }
    void Check()
    {
        RaycastHit2D left = Physics2D.Raycast(Left.position, Vector2.left,distance);
        RaycastHit2D right = Physics2D.Raycast(Right.position, Vector2.right, distance);
        RaycastHit2D up = Physics2D.Raycast(Up.position, Vector2.up, distance);
        RaycastHit2D down = Physics2D.Raycast(Down.position, Vector2.down, distance);
        if (left.collider != null && right.collider !=null)
        {         
            GameObject boxLeft = left.collider.gameObject;
            GameObject boxRight = right.collider.gameObject;
            if (boxLeft.GetComponent<AText>() != null && boxRight.GetComponent<BText>() != null)
            {
                AText aText = boxLeft.GetComponent<AText>();
                BText bText = boxRight.GetComponent<BText>();
                Debug.Log(aText.text + " is " + bText.text);
            }
        }
        if (up.collider != null && down.collider != null)
        {
            GameObject boxUp = up.collider.gameObject;
            GameObject boxDown = down.collider.gameObject;
       
            if (boxUp.GetComponent<AText>() != null && boxDown.GetComponent<BText>() != null)
            {
                AText aText = boxUp.GetComponent<AText>();
                BText bText = boxDown.GetComponent<BText>();
                Debug.Log(aText.text + " is " + bText.text);
            }
        }
       
    }
   
}
