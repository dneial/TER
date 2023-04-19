using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpaceColonization
{
    private Vector3 startPos = new Vector3(0, 0, 0);
    private Vector3 center = new Vector3(0, 10, 0);

    private float leaf_kill_distance;
    private float leaf_influence_radius;
    private int influence_points;

    private float radius = 10f;

    private List<Leaf> leaves { get; set; } = new List<Leaf>();

    private List<Node> nodes = new List<Node>();

    private Node root_node = new Node(Vector3.zero, Vector3.up);

    private CrownVolume volume;

    private Mesh vol;


    public bool done { get; set; } = false;
    public int steps { get; set; } = 0;

    public SpaceColonization(
        float leaf_kill_distance = 1f, 
        float leaf_influence_radius = 9f, 
        int nb_points = 100,
        Mesh vol = null
        )
    {
        
        this.leaf_kill_distance = leaf_kill_distance;
        this.leaf_influence_radius = leaf_influence_radius;
        this.influence_points = nb_points;

        this.nodes.Add(this.root_node);

        this.vol = vol;
        
        this.GenerateVolume(32, 15f, 10f);

    }

    public void start(){
        this.leaves.AddRange(this.PlaceLeaves(this.influence_points));
        this.GenerateRoot();
    }
    

    public void Generate()
    {
        this.start();
        while(!this.done) {
            this.Grow();
        }
        this.NormalizeThickness();
    }




    private void GenerateVolume(int nb_points, float radius, float height) {
        // Mesh mesh = CrownVolume.GetConeMesh(nb_points, radius, height);
        // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // go.GetComponent<MeshFilter>().mesh = mesh;

        // Vector3[] points = this.PPWV(mesh);

        // foreach(Vector3 point in points) {
        //     GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //     obj.transform.position = point;
        // }


        this.volume = CrownVolume.GetCone(nb_points, nb_points/1.4f, nb_points/1.2f);
    }

    // place x number of points around the center
    private List<Leaf> PlaceLeaves(int nbPoints) {

        List<Leaf> leaves = new List<Leaf>();

        // Vector3[] points = this.PlacePointsWithinVolume(nbPoints, this.volume);

        Vector3[] points = this.PPWV(nbPoints);


        for (int i = 0; i < nbPoints; i++) {
            // random between 0 and 1 and test if greater than 0.5
            // bool repulse = Random.value > 0.75f;
            Vector3 position = points[i];
            Leaf leaf = new Leaf(i, position, this.leaf_kill_distance, this.leaf_influence_radius);
            // leaf.inversed = repulse;
            leaves.Add(leaf);
        }


        return leaves;
    }

    private Vector3 GetDirection(Vector3 position, Vector3 closest) {
        Vector3 direction = closest - position;
        direction.Normalize();
        return direction;
    }

    public (List<Leaf>, List<Node>) Grow() 
    {
        this.AttractNodes();
        List<Leaf> dropped_leaves = this.DropLeaves();
        List<Node> new_nodes = this.GrowNodes();
        this.steps++;

        return (dropped_leaves, new_nodes);
    }


    // Pour chaque feuille, on cherche le noeud le plus proche. 
    // Si le noeud est dans le rayon d'influence, on change sa direction vers la feuille.
    private void AttractNodes()
    {
        Leaf leaf;
        Node closest = null;
        for(int i = 0; i < this.leaves.Count; i++)
        {
            leaf = this.leaves[i];
            closest = leaf.FindClosestNode(this.nodes);
            if (closest != null) {
                if(leaf.inversed)
                    closest.direction -= this.GetDirection(closest.position, leaf.position);
                else
                    closest.direction += this.GetDirection(closest.position, leaf.position);
                closest.influences++;
                closest.isInfluenced = true;
            }
        }
    }


    // Pour chaque noeud influencé, on crée une nouvelle branche positionnée dans la direction du noeud. 
    // Le noeud fils hérite de la direction du noeud père.
    private List<Node> GrowNodes()
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

                growThickness(newNode);

                node.direction = node.originalDirection;
            }
            
        }

        this.nodes.AddRange(newNodes);
        return newNodes;

    }

    private void growThickness(Node node){
        if(node.parent != null){
            node.parent.thickness++;
            growThickness(node.parent);
        }
    }

    // Retire les feuilles qui ont été atteintes par un noeud.
    private List<Leaf> DropLeaves() {
        List<Leaf> to_remove = new List<Leaf>();

        foreach (Leaf leaf in this.leaves) {
            if (leaf.reached) {
                to_remove.Add(leaf);
            }
        }

        this.leaves.RemoveAll(leaf => leaf.reached);

        this.done = this.leaves.Count == 0;

        return to_remove;
    }

    // Première étape de la génération: tant que le noeud racine n'est pas influencé par une feuille, on avance d'un pas dans la direction définie par défaut.
    private Node GenerateRoot()
    {
        this.root_node = this.nodes[0];
        Node root = this.root_node;

        List<Leaf> influenceSet = this.FindInfluencingLeaves(root.position);
        root.isInfluenced = influenceSet.Count > 0;

        while(!root.isInfluenced) {
            root = root.GetNext();
            influenceSet = this.FindInfluencingLeaves(root.position);
            root.isInfluenced = influenceSet.Count > 0;
            growThickness(root);
            this.nodes.Add(root);
        }

        return root;
    }

    public void NormalizeThickness(){
        float thickness = this.root_node.thickness;
        foreach(Node node in this.nodes) {
            node.thickness /= thickness;
        }
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


    // Generate random points within the given volume
    private Vector3[] PlacePointsWithinVolume(int nbPoints, CrownVolume volume)
    {

        Vector3[] points = new Vector3[nbPoints];

        for (int i = 0; i < nbPoints; i++)
        {
            // Generate a random point on the surface of the volume
            Vector3 randomPoint = Vector3.zero;
            int randomIndex = Random.Range(0, volume.segments);
            randomPoint.x = volume.vertices[randomIndex + 1].x;
            randomPoint.z = volume.vertices[randomIndex + 1].z;
            randomPoint.y = Random.Range(volume.height/2f, volume.height);

            // Project the random point onto the inside of the volume
            float t = 1f - Mathf.Abs(randomPoint.y / volume.height);
            randomPoint.x *= t;
            randomPoint.z *= t;

            points[i] = randomPoint;
        }

        return points;
    }

    private Vector3[] PPWV(int nb_points)
    {

        Mesh mesh = this.vol;
        Vector3[] points = new Vector3[nb_points];

        Vector3 min = mesh.bounds.min;
        Vector3 max = mesh.bounds.max;

        Debug.Log("min: " + min);
        Debug.Log("max: " + max);


        for (int i = 0; i < nb_points; i++)
        {
            Vector3 randomPoint = Vector3.zero;
            randomPoint.x = Random.Range(min.x, max.x);
            randomPoint.y = Random.Range(min.y, max.y);
            randomPoint.z = Random.Range(min.z, max.z);

            points[i] = randomPoint;
        }

        return points;
    }

    public List<Node> GetNodes() {
        return this.nodes;
    }

    public List<Leaf> GetLeaves() {
        return this.leaves;
    }
}
