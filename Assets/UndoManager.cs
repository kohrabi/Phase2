using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class UndoType
{
    public GameObject TargetObject;
    public Vector3 PrevPosition;


    public UndoType(GameObject targetObject, Vector3 prevPosition)
    {
        this.TargetObject = targetObject;
        this.PrevPosition = prevPosition;
    }
}

public class UndoManager : MonoBehaviour
{

    public static UndoManager Instance
    {
        get;
        private set;
    }


    Stack<List<UndoType>> undoObjects = new ();

    void Awake()
    {
        undoObjects.Clear();
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Debug.LogError("WTF there are more than one UndoManager in this");
    }

    private void Start()
    {
        NextTurn();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            Undo();
    }

    void Undo()
    {
        if (undoObjects.Count <= 0)
            return;
        // Undo next turn which is empty 
        if (undoObjects.Count > 1)
            undoObjects.Pop();
        var undoObjs = undoObjects.Peek();
        foreach (UndoType undoObj in undoObjs)
        {
            // Todo disable object not destroy it
            if (undoObj.TargetObject == null)
                continue;
            if (undoObj.TargetObject.TryGetComponent<GridMoveComponent>(out var grid))
                grid.UndoMove(undoObj.PrevPosition - undoObj.TargetObject.transform.position);
        }

        undoObjects.Pop();
        NextTurn();
    }

    public void NextTurn()
    {
        undoObjects.Push(new List<UndoType>());
    }

    public void AddToCurrentUndo(GameObject obj, Vector3 pos)
    {
        if (undoObjects.Count <= 0)
            NextTurn();
        undoObjects.Peek().Add(new UndoType(obj, pos));
    }

}