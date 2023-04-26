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

    private List<Leaf> leaves { get; set; } = new List<Leaf>();

    private List<Node> nodes = new List<Node>();

    private Node root_node = new Node(Vector3.zero, Vector3.up);

    private CrownVolume volume;

    private Bounds vol;


    public bool done { get; set; } = false;
    public int steps { get; set; } = 0;

    public SpaceColonization(
        Bounds vol,
        float leaf_kill_distance = 1f, 
        float leaf_influence_radius = 9f, 
        int nb_points = 100
        )
    {
        
        this.leaf_kill_distance = leaf_kill_distance;
        this.leaf_influence_radius = leaf_influence_radius;
        this.influence_points = nb_points;

        this.nodes.Add(this.root_node);

        this.vol = vol;
    }

    public void start(){
        this.leaves.AddRange(this.PlaceLeaves(this.influence_points));
        this.GenerateRoot();
    }
    

    public void Generate(int max_iterations)
    {
        this.start();
        while(!this.done && this.steps < max_iterations) {
            this.leaves.AddRange(this.PlaceLeaves(5));
            this.Grow();
        }
        this.NormalizeThickness();
    }


/*

    public IEnumerable<List<Leaf>> Generate(int max_iterations)
    {
        this.start();
        while(!this.done && this.steps < max_iterations) {
            this.Grow();
            List<Leaf> newLeaves = this.PlaceLeaves(5); 
            this.leaves.AddRange(newLeaves);
            yield return newLeaves;
        }
        this.NormalizeThickness();
    }

*/

    // place x number of points around the center
    private List<Leaf> PlaceLeaves(int nbPoints) {

        List<Leaf> leaves = new List<Leaf>();

        Vector3[] points = this.PlacePoints(nbPoints);


        for (int i = 0; i < points.Length; i++) {
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


   private Vector3[] PlacePoints(int nb_points)
    {
        List<Vector3> points = new List<Vector3>();

        points.AddRange(this.PlacePointsInMesh(nb_points));

        while(points.Count < nb_points)
        {
            points.AddRange(this.PlacePointsInMesh(nb_points - points.Count));
        }

        return points.ToArray();
    }

    private Vector3[] PlacePointsInMesh(int nb_points)
    {

        Vector3[] points = new Vector3[nb_points];

        Vector3 min = this.vol.min;
        Vector3 max = this.vol.max;

        for (int i = 0; i < nb_points; i++)
        {
            Vector3 randomPoint = this.vol.center;
            randomPoint.x = Random.Range(min.x, max.x);
            randomPoint.y = Random.Range(min.y, max.y);
            randomPoint.z = Random.Range(min.z, max.z);

            points[i] = randomPoint;
        }


        points = this.TestPoints(points);

        return points;
    }


    private Vector3[] TestPoints(Vector3[] points)
    {

        List<Vector3> newPoints = new List<Vector3>();

        foreach(Vector3 p in points)
        {
            Vector3 depart = p;
            RaycastHit[] hits;

            Vector3[] directions = 
            new Vector3[] { 
                Vector3.right, 
                Vector3.forward, 
                Vector3.left, 
                Vector3.back,
                Vector3.down,
                new Vector3(1, 1, 0),
                new Vector3(1, 1, 1),
                new Vector3(1, 1, -1),
                new Vector3(1, -1, 0),
                new Vector3(1, -1, 1),
                new Vector3(1, -1, -1),
                new Vector3(-1, 1, 0),
                new Vector3(-1, 1, 1),
                new Vector3(-1, 1, -1),
                new Vector3(-1, -1, 0),
                new Vector3(-1, -1, 1),
                new Vector3(-1, -1, -1)
            };


            int collisions = 0;

            foreach(Vector3 dir in directions)
            {
                hits = Physics.RaycastAll(depart, dir, 100.0f);
                if(hits.Length > 0)
                {
                    collisions++;
                    break;
                }
            }

            if(collisions == 0)
            {
                newPoints.Add(p);
            }
        }

        return newPoints.ToArray();
    }


    public List<Node> GetNodes() {
        return this.nodes;
    }

    public List<Leaf> GetLeaves() {
        return this.leaves;
    }
}
