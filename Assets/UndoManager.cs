using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class UndoType
{
    public GameObject TargetObject;
    public Vector3 PrevPosition;
    public bool DidReplaceObject = false;
    public GameObject OldReplaceObjects = null;

    public UndoType(GameObject targetObject, Vector3 prevPosition, bool didReplace = false, GameObject oldReplaceObj = null)
    {
        this.TargetObject = targetObject;
        this.PrevPosition = prevPosition;
        this.DidReplaceObject = didReplace;
        this.OldReplaceObjects = oldReplaceObj;
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
        Debug.Log(undoObjects.Count);
        if (Input.GetKey(KeyCode.Z))
            Undo();
    }

    void Undo()
    {
        if (undoObjects.Count <= 0)
            return;
        if (!GridMoveComponent.CanMove)
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
            if (undoObj.DidReplaceObject)
            {
                undoObj.OldReplaceObjects.SetActive(true);
                Destroy(undoObj.TargetObject);
                continue;
            }
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

    public void AddToCurrentUndo(GameObject obj, Vector3 pos, bool didReplace = false, GameObject oldReplaceObj = null)
    {
        if (undoObjects.Count <= 0)
            NextTurn();
        undoObjects.Peek().Add(new UndoType(obj, pos, didReplace, oldReplaceObj));
    }

}
