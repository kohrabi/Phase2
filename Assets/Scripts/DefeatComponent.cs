using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DefeatComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            transform.position += Vector3.up;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //Nhan va cham khi player vao box defeat
        if (collision.gameObject.TryGetComponent<Player>(out var playercomponent))
        {        
            GameObject player = collision.gameObject;
            player.SetActive(false);

            //Thay doi vi tri cua player sang ben canh box defeat
            Vector2 currentPosition = player.transform.position;
            float newX = 0, newY = 0;
            if (player.transform.position.x >= transform.position.x)
                newX = Mathf.Round(currentPosition.x);
            if (player.transform.position.x < transform.position.x)
                newX = Mathf.Round(currentPosition.x - 1);
            if (player.transform.position.y >= transform.position.y)
                newY = Mathf.Round(currentPosition.y);
            if (player.transform.position.y < transform.position.y)
                newY = Mathf.Round(currentPosition.y - 1);
            player.transform.position = new Vector2(newX + 0.5f, newY + 0.5f);

            //Thay doi movecurr, movetarget
            playercomponent.GridMove.SetPosition(player.transform.position);

            //Object rong thay the 
            GameObject temp = new GameObject();
            temp.transform.position = player.transform.position;
            UndoManager.Instance.AddToCurrentUndo(temp, Vector3.zero, true, player);
        }
    }

}
