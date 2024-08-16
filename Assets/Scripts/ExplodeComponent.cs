using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeComponent : MonoBehaviour
{
    bool start = false;
    
    // Start is called before the first frame update
    void Start()
    {
        start = true;
    }

    private void Awake()
    { 
        Collider2D[] boxaround = Physics2D.OverlapBoxAll(transform.position, transform.localScale * 3, 0);  //localscale = 1/2 box = 1/3 radius
        foreach(Collider2D c in boxaround)
        {
            Boom(c.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Boom(GameObject explodeobj)
    {       
        explodeobj.SetActive(false); 
        //Object rong thay the 
        GameObject temp = new GameObject();
        temp.transform.position = explodeobj.transform.position;
        UndoManager.Instance.AddToCurrentUndo(temp, Vector3.zero, true, explodeobj);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(start)
        Gizmos.DrawWireCube(transform.position, transform.localScale * 3);
    }

}
