using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum Direction
{
    up,
    down,
    left,
    right,
    none
}
public class MapBlock : MonoBehaviour
{
    [SerializeField] private List<GameObject> doorList;
    public Vector3Int posID { get; private set; }
    public Direction direction { get; private set; }

    private void Start()
    {
        var position = transform.position;
        posID = new Vector3Int((int)position.x, (int)position.y, (int)position.z);
    }

    public void SetBlock(Direction dir, List<Direction> toOpen)
    {
        direction = dir;
        foreach (var door in doorList)
            door.SetActive(false);
        foreach (var openDir in toOpen)
            doorList[(int)openDir].SetActive(true);
    }
}
