using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private static TurnManager _instance;
    public static TurnManager Instance => _instance;
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }

    private ChaseComponent[] _chaseObjects;


    private void Start()
    {
        Player.PlayerMove += ChaseObjectMove;
    }

    private void OnDestroy()
    {
        Player.PlayerMove -= ChaseObjectMove;
    }

    private void ChaseObjectMove()
    {
        _chaseObjects = FindObjectsOfType<ChaseComponent>();
        if (_chaseObjects == null)
            return;
        foreach (ChaseComponent chaseOb in _chaseObjects)
        {
            chaseOb.Chase();
        }
    }
}
