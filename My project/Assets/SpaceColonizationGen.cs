using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class SpaceColonizationGen : MonoBehaviour
{
    private Vector3 startPos = new Vector3(0, 0, 0);
    private Vector3 center = new Vector3(0, 10, 0);

    public float leaf_kill_distance = 1f;
    public float leaf_influence_radius = 9f;

    private float radius = 10f;
    private float step = 1f;

    private List<Leaf> leaves;
    private List<GameObject> node_links = new List<GameObject>();

    private List<Node> nodes = new List<Node>() {
        new Node(Vector3.zero, Vector3.up)
    };

    private bool started = false;

    public SpaceColonizationGen() {
    }

    public void Start()
    {
        this.leaves = this.PlaceLeaves(100);
        this.GenerateRoot();
        
    }


    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            while(this.leaves.Count > 0) this.Generate();
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(this.leaves.Count > 0) this.Generate();
            else Debug.Log("No more leaves");
        }

        
    }


    // place x number of points around the center
    private List<Leaf> PlaceLeaves(int nbPoints) {
        List<Leaf> leaves = new List<Leaf>();
        Vector3[] points = this.GetPointsWithinCone(nbPoints);

        for (int i = 0; i < nbPoints; i++) {
            Vector3 position = points[i];
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Leaf " + i;
            sphere.transform.localScale = new Vector3(1f, 1f, 1f);
            sphere.transform.position = position;
            sphere.GetComponent<Renderer>().material.color = Color.red;
            Leaf leaf = new Leaf(i, position, this.leaf_kill_distance, this.leaf_influence_radius);
            leaf.gameobject = sphere;
            leaves.Add(leaf);
        }

        GameObject root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        root.name = "Root";
        root.transform.position = this.startPos;

        return leaves;
    }

    private Vector3 GetDirection(Vector3 position, Vector3 closest) {
        Vector3 direction = closest - position;
        direction.Normalize();
        return direction;
    }

    private void Generate() 
    {

        this.AttractNodes();

        this.DropLeaves();

        this.GrowNodes();
       
    }

    private void AttractNodes()
    {
    
        Leaf leaf;
        Node closest = null;
    
        for(int i = 0; i < this.leaves.Count; i++)
        {
            leaf = this.leaves[i];
            closest = leaf.FindClosestNode(this.nodes);

            if (closest != null) {
                closest.direction += this.GetDirection(closest.position, leaf.position);
                closest.influences++;
                closest.isInfluenced = true;
            }
        }
    }

    private void GrowNodes()
    {
        List<Node> newNodes = new List<Node>();
        Node node, newNode;
        for(int i = 0; i < this.nodes.Count; i++)
        {
            node = this.nodes[i];
            if(node.isInfluenced) {
                node.direction /= node.influences + 1;
                node.direction.Normalize();

                node.influences = 0;
                node.isInfluenced = false;
                
                newNode = node.GetNext();
                newNodes.Add(newNode);

                node.direction = node.originalDirection;
            }
        }

        this.nodes.AddRange(newNodes);
        this.LinkNodes(newNodes);

        newNodes.Clear();
    }

    private Node GenerateRoot()
    {
        Node root = this.nodes[0];

        List<Leaf> influenceSet = this.FindInfluencingLeaves(root.position);
        root.isInfluenced = influenceSet.Count > 0;

        while(!root.isInfluenced) {
            root = root.GetNext();
            influenceSet = this.FindInfluencingLeaves(root.position);
            root.isInfluenced = influenceSet.Count > 0;
            this.nodes.Add(root);
        }

        this.LinkNodes(this.nodes);
        return root;
    }

    private List<Leaf> FindInfluencingLeaves(Vector3 position)
    {
        List<Leaf> influencingLeaves = new List<Leaf>();
        foreach(Leaf leaf in this.leaves) {
            if(Vector3.Distance(position, leaf.position) < leaf.influence_radius) {
                influencingLeaves.Add(leaf);
            }
        }

        return influencingLeaves;
    }



    private void DropLeaves() {

        List<Leaf> to_remove = new List<Leaf>();

        foreach (Leaf leaf in this.leaves) {
            if (leaf.reached) {
                to_remove.Add(leaf);
            }
        }

        foreach (Leaf leaf in to_remove) {
            this.leaves.Remove(leaf);
            Destroy(leaf.gameobject);
        }

        
    }

    private void PlaceSphereNodes(List<Node> nodes) {
        foreach(Node node in nodes){
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Node " + node.id;
            sphere.transform.localScale = new Vector3(1f, 1f, 1f);
            sphere.transform.position = node.position;
        }
    }

    private void LinkNodes(List<Node> nodes) 
    {
        foreach(Node node in nodes){
            if(node.parent != null){
                GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cylinder.name = "Node(" + node.parent.id + ", " + node.id + ")";
                cylinder.transform.position = (node.position + node.parent.position) / 2f;
                cylinder.transform.localScale = new Vector3(0.1f, 0.1f, Vector3.Distance(node.position, node.parent.position));
                cylinder.transform.LookAt(node.position);
                this.node_links.Add(cylinder);
            }
        }
    }


    private Vector3[] GetPointsWithinCone(int nbPoints)
    {
        // Define the parameters of the cone
        int segments = 32;
        float height = 15f;
        float radius = 10f;

        // Create a new mesh
        Mesh mesh = new Mesh();

        // Define the vertices and triangles of the cone
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        // Define the top vertex
        vertices[0] = new Vector3(0f, height, 0f);

        // Define the vertices around the base of the cone
        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.PI * 2f / segments * i;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            vertices[i] = new Vector3(x, height*2, z);
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

        // Generate random points within the volume of the cone
        Vector3[] points = new Vector3[nbPoints];

        for (int i = 0; i < nbPoints; i++)
        {
            // Generate a random point on the surface of the cone
            Vector3 randomPoint = Vector3.zero;
            int randomIndex = Random.Range(0, segments);
            randomPoint.x = vertices[randomIndex + 1].x;
            randomPoint.z = vertices[randomIndex + 1].z;
            randomPoint.y = Random.Range(height, height*1.5f);

            // Project the random point onto the inside of the cone
            float t = 1f - randomPoint.y / height;
            randomPoint.x *= t;
            randomPoint.z *= t;

            points[i] = randomPoint;
        }

        Destroy(mesh);
        return points;
    }


    public Vector3[] GetPointsWithinConicalCrown(int nbPoints)
    {
        // Define the parameters of the cone
        int segments = 64;
        float height = 15f;
        float radius = 10f;
        float crownHeight = 5f;

        // Create a new mesh
        Mesh mesh = new Mesh();

        // Define the vertices and triangles of the cone
        Vector3[] vertices = new Vector3[(segments + 1) * 2];
        int[] triangles = new int[segments * 6];

        // Define the top vertices
        vertices[0] = new Vector3(0f, height, 0f);
        vertices[segments + 1] = new Vector3(0f, height - crownHeight, 0f);

        // Define the vertices around the base of the cone
        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.PI * 2f / segments * i;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            vertices[i] = new Vector3(x, height, z);
            vertices[i + segments + 1] = new Vector3(x, height - crownHeight, z);
        }

        // Define the triangles of the cone
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 6] = i + 1;
            triangles[i * 6 + 1] = segments + 1 + i + 1;
            triangles[i * 6 + 2] = i + 2;

            triangles[i * 6 + 3] = i + 2;
            triangles[i * 6 + 4] = segments + 1 + i + 1;
            triangles[i * 6 + 5] = segments + 1 + i + 2;
        }

        // Set the last triangle to wrap around to the first vertex
        triangles[segments * 6 - 1] = 1;

        // Apply the vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Calculate the normals of the mesh
        mesh.RecalculateNormals();



        // Generate random points within the volume of the cone
        Vector3[] points = new Vector3[nbPoints];

        for (int i = 0; i < nbPoints; i++)
        {
            // Generate a random point on the surface of the cone
            Vector3 randomPoint = Vector3.zero;
            int randomIndex = Random.Range(0, segments);
            randomPoint.x = vertices[randomIndex + 1].x;
            randomPoint.z = vertices[randomIndex + 1].z;
            randomPoint.y = Random.Range(0f, height);

            // Project the random point onto the inside of the cone
            float t = 1f - randomPoint.y / height;
            randomPoint.x *= t;
            randomPoint.z *= t;

            points[i] = randomPoint;
        }
        Destroy(mesh);
        return points;
    }
}