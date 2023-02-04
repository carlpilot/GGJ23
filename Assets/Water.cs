using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public float deflectionHeight = 2f;
    public float noiseScale = 1f;
    public float noiseSpeed = 1f;
    Mesh mesh;
    float timer;
    float timer2 = 1000;

    void Awake()
    {
        var mf = GetComponent<MeshFilter>();
        mesh = mf.mesh;//Instantiate(mf.mesh);
        //mf.mesh = mesh;
    }

    void Update()
    {
        timer += Time.deltaTime*noiseSpeed;
        timer2 -= Time.deltaTime*noiseSpeed;
        Vector3[] verts = mesh.vertices;
        for (int v = 0; v < verts.Length; v++)
        {
            var p = verts[v];
            var h = 0.5f * (Mathf.PerlinNoise(p.x/noiseScale+timer, p.y/noiseScale+timer) + Mathf.PerlinNoise(p.x/noiseScale+timer2, p.y/noiseScale-timer2)) - 0.5f;
            h *= deflectionHeight;
            verts[v] = new Vector3(p.x, p.y, h);
        }
        mesh.vertices = verts;
    }
}
