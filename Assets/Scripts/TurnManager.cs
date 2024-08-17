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
    public List<GameObject> Players { get; private set; }
    public List<Vector2> OccupiedPos = new List<Vector2>();


    private void Start()
    {
        InitPlayerList();
        GridMoveComponent.Moved = false;
        GridMoveComponent.CanMove = true;
        Player.PlayerMove += ChaseObjectMove;
    }

    private void FixedUpdate()
    {
        OccupiedPos.Clear();
    }

    private void OnDestroy()
    {
        Player.PlayerMove -= ChaseObjectMove;
    }

    private void InitPlayerList()
    {
        Players = new List<GameObject>();
        Player[] players = GameObject.FindObjectsOfType<Player>();
        foreach (var player in players)
        {
            Players.Add(player.gameObject);
        }
    }

    public void ChangePlayerList(GameObject[] pList)
    {
        Players = new List<GameObject>(pList);
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
