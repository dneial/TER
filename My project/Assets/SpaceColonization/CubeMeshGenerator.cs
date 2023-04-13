using UnityEngine;

public class CubeMeshGenerator : MonoBehaviour
{
    public int numSpheres = 100; // Number of spheres to generate
    public float sphereRadius = 1.0f; // Radius of each sphere
    public GameObject spherePrefab; // Prefab of the sphere
    public Vector3 cubeSize = Vector3.one; // Size of the cube

    void Start()
    {
        GenerateCubeMesh();
    }

    void GenerateCubeMesh()
    {
        for (int i = 0; i < numSpheres; i++)
        {
            Vector3 position = new Vector3(Random.Range(-cubeSize.x / 2, cubeSize.x / 2),
                                          Random.Range(-cubeSize.y / 2, cubeSize.y / 2),
                                          Random.Range(-cubeSize.z / 2, cubeSize.z / 2)); // Generate a random position inside the cube

            GameObject sphere = Instantiate(spherePrefab, transform.position + position, Quaternion.identity, transform); // Instantiate a sphere prefab at the calculated position
            sphere.transform.localScale = Vector3.one * sphereRadius * 2.0f; // Set the scale of the sphere based on its radius
        }
    }
}
