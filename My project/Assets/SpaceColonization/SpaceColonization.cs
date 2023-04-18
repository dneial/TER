using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class SpaceColonization
{
    private Vector3 startPos = new Vector3(0, 0, 0);
    private Vector3 center = new Vector3(0, 10, 0);

    private float leaf_kill_distance = 1f;
    private float leaf_influence_radius = 9f;

    private float radius = 10f;

    private List<Leaf> leaves { get; set; }
    //private List<GameObject> node_links = new List<GameObject>();

    private List<Node> nodes = new List<Node>();

    private Node root_node = new Node(Vector3.zero, Vector3.up);

    //private GameObject root;

    private CrownVolume volume;


    public bool done { get; set; } = false;
    public int steps { get; set; } = 0;

    public SpaceColonization(float leaf_kill_distance = 1f, float leaf_influence_radius = 9f, int nbPoints = 100)
    {
        
        this.leaf_kill_distance = leaf_kill_distance;
        this.leaf_influence_radius = leaf_influence_radius;

        this.root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        root.name = "Root";
        root.transform.position = this.startPos;

        this.nodes.Add(this.root_node);

        this.leaves = this.PlaceLeaves(nbPoints);

        this.GenerateRoot();
    }
    
    // place x number of points around the center
    private List<Leaf> PlaceLeaves(int nbPoints) {
        List<Leaf> leaves = new List<Leaf>();

        this.volume = CrownVolume.GetCone(32, 15f, 10f);
        
        Vector3[] points = this.PlacePointsWithinVolume(nbPoints, this.volume);


        for (int i = 0; i < nbPoints; i++) {
            Vector3 position = points[i];
            // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // sphere.name = "Leaf " + i;
            // sphere.transform.localScale = new Vector3(1f, 1f, 1f);
            // sphere.transform.position = position;
            // sphere.transform.parent = this.root.transform;
            Leaf leaf = new Leaf(i, position, this.leaf_kill_distance, this.leaf_influence_radius);
            //leaf.gameobject = sphere;
            leaves.Add(leaf);
        }


        return leaves;
    }

    private Vector3 GetDirection(Vector3 position, Vector3 closest) {
        Vector3 direction = closest - position;
        direction.Normalize();
        return direction;
    }

    public void Generate() 
    {

        this.AttractNodes();
        this.DropLeaves();
        this.GrowNodes();
        this.steps++;

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
                closest.direction += this.GetDirection(closest.position, leaf.position);
                closest.influences++;
                closest.isInfluenced = true;
            }
        }
    }


    // Pour chaque noeud influencé, on crée une nouvelle branche positionnée dans la direction du noeud. 
    // Le noeud fils hérite de la direction du noeud père.
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

                growThickness(newNode);

                node.direction = node.originalDirection;
            }
            
        }

        this.nodes.AddRange(newNodes);
        //this.NormalizeThickness();
        //this.LinkNodes(newNodes);

    }

    private void growThickness(Node node){
        if(node.parent != null){
            node.parent.thickness++;
            growThickness(node.parent);
        }
    }

    // Retire les feuilles qui ont été atteintes par un noeud.
    private void DropLeaves() {
        List<Leaf> to_remove = new List<Leaf>();

        foreach (Leaf leaf in this.leaves) {
            if (leaf.reached) {
                to_remove.Add(leaf);
            }
        }

        // foreach (Leaf leaf in to_remove) {
        //     this.leaves.Remove(leaf);
        //     UnityEngine.Object.DestroyImmediate(leaf.gameobject);
        // }

        this.done = this.leaves.Count == 0;
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

        //this.LinkNodes(this.nodes);
        return root;
    }

    private void NormalizeThickness(){
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



    // private void PlaceSphereNodes(List<Node> nodes) {
    //     foreach(Node node in nodes){
    //         GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //         sphere.name = "Node " + node.id;
    //         sphere.transform.localScale = new Vector3(1f, 1f, 1f);
    //         sphere.transform.position = node.position;
    //     }
    // }




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
}
