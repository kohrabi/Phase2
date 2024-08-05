using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PushableComponent))]
public class BText : MonoBehaviour, INameText
{
    public string Text;
    public string ComponentName;
    public Type ComponentType;

    // Start is called before the first frame update
    void Start()
    {
        ComponentType = Type.GetType(ComponentName);
    }
}
