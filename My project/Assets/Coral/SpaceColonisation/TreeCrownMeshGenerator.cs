using UnityEngine;

public class TreeCrownMeshGenerator : MonoBehaviour
{
    public int numLevels = 5; // Number of hierarchical levels in the tree
    public int numSpheresPerLevel = 4; // Number of spheres per level
    public float sphereRadius = 1.0f; // Radius of each sphere
    public float sphereSpacing = 1.0f; // Spacing between spheres
    public GameObject spherePrefab; // Prefab of the sphere

    void Start()
    {
        GenerateTreeCrownMesh();
    }

    void GenerateTreeCrownMesh()
    {
        float yOffset = 0.0f; // Vertical offset for each level

        for (int i = 0; i < numLevels; i++)
        {
            int numSpheresInLevel =(int) Mathf.Pow(numSpheresPerLevel, i);
            //int numSpheresInLevel = Mathf.FloorToInt(2 * Mathf.PI * levelRadius / (2 * sphereRadius + sphereSpacing)); // Calculate number of spheres in current level
            float levelRadius = i * sphereSpacing * 2.0f; // Calculate radius of the level

            for (int j = 0; j < numSpheresInLevel; j++)
            {
                float angle = j * 360.0f / numSpheresInLevel; // Calculate angle for each sphere
                Vector3 position = Quaternion.Euler(0.0f, angle, 0.0f) * Vector3.forward * levelRadius; // Calculate position of each sphere based on angle and radius
                position.y = yOffset; // Set the vertical position of the sphere in the current level
                GameObject sphere = Instantiate(spherePrefab, transform.position + position, Quaternion.identity, transform); // Instantiate a sphere prefab at the calculated position
                sphere.transform.localScale = Vector3.one * sphereRadius * 2.0f; // Set the scale of the sphere based on its radius
            }

            yOffset += sphereSpacing; // Increment the vertical offset for the next level
        }
    }
}
