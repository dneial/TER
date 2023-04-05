using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class CrownVolume 
{

    public int segments { get; set; }
    public float height { get; set; }
    public Vector3[] vertices { get; set; }

    private CrownVolume(int segments, float height, Vector3[] vertices)
    {
        this.segments = segments;
        this.height = height;
        this.vertices = vertices;
    }

    public static CrownVolume GetCone(int segments, float height, float radius)
    {
        // Create a new mesh
        Mesh mesh = new Mesh();

        // Define the vertices and triangles of the cone
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        // Define the top vertex
        vertices[0] = new Vector3(0f, height/2, 0f);

        // Define the vertices around the base of the cone
        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.PI * 2f / segments * i;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            vertices[i] = new Vector3(x, height/2, z);
        }

        // Define the center of the base
        vertices[segments + 1] = new Vector3(0f, height, 0f);

        // Define the triangles of the cone
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = i + 1;
            triangles[i * 3 + 1] = segments + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        // Set the last triangle to wrap around to the first vertex
        triangles[segments * 3 - 1] = 1;

        // Apply the vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Calculate the normals of the mesh
        mesh.RecalculateNormals();

        UnityEngine.Object.DestroyImmediate(mesh);

        return new CrownVolume(segments, height, vertices);
    }
}