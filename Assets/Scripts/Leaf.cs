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
    
    public Leaf(int x, int z, int w, int d, int s)
    {
        xpos = x;
        zpos = z;
        width = w;
        depth = d;
        scale = s;
    }

    public void Draw()
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
    }
}
