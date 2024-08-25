using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDungeon : MonoBehaviour
{
    [SerializeField] private int mapWidth = 50;
    [SerializeField] private int mapDepth= 50;
    [SerializeField] private int scale = 2;
    private Leaf root;

    private void Start()
    {
        root = new Leaf(0, 0, mapWidth, mapDepth, scale);
        //root.Draw();

        int halfWidth = mapWidth / 2;
        
        Leaf left = root = new Leaf(0, 0, halfWidth, mapDepth, scale);
        Leaf right = root = new Leaf(halfWidth, 0, halfWidth, mapDepth, scale);
        left.Draw();
        right.Draw();
    }
}
