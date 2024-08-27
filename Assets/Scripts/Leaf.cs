using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf
{
    private int xpos;
    private int zpos;
    private int width;
    private int depth;
    private int scale;
    private int roomMin = 5;

    public Leaf leftChild;
    public Leaf rightChild;
    
    public Leaf(int x, int z, int w, int d, int s)
    {
        xpos = x;
        zpos = z;
        width = w;
        depth = d;
        scale = s;
    }

    public bool Split()
    {
        if (width <= roomMin || depth <= roomMin) return false;

        bool splitHorizontal = Random.Range(0,100) > 50;

        if (width > depth && (width / (float)depth) >= 1.2f)
        {
            splitHorizontal = false; // Split vertically
        }
        else if (depth > width && (depth / (float)width) >= 1.2f)
        {
            splitHorizontal = true; // Split horizontally
        }

        int max = (splitHorizontal ? depth : width) - roomMin;

        if (max <= roomMin) return false;

        int splitPos = Random.Range(roomMin, max);

        if (splitHorizontal)
        {
            leftChild = new Leaf(xpos, zpos, width, splitPos, scale);
            rightChild = new Leaf(xpos, zpos + splitPos, width, depth - splitPos, scale);
        }
        else
        {
            leftChild = new Leaf(xpos, zpos, splitPos, depth, scale);
            rightChild = new Leaf(xpos + splitPos, zpos, width - splitPos, depth, scale);
        }

        return true;
    }

    public void Draw(byte[,] map)
    {
        Color c = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        for (int x = xpos; x < width + xpos; x++)
        {
            for (int z = zpos; z < depth + zpos; z++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(x * scale, 0, z * scale);
                cube.transform.localScale = new Vector3(scale, scale, scale);
                cube.GetComponent<Renderer>().material.SetColor("_Color", c);
            }
        }

        for (int x = xpos + 1; x < width + xpos - 1; x++)
        {
            for (int z = zpos + 1; z < depth + zpos - 1; z++)
            {
                map[x, z] = 0;
            }
        }
    }
}
