using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace demo {
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

        private Bounds vol;

        private GameObject root;


        public bool done { get; set; } = false;
        public int steps { get; set; } = 0;

        public SpaceColonization(
            Bounds vol,
            GameObject root,
            float leaf_kill_distance = 1f, 
            float leaf_influence_radius = 9f, 
            int nb_points = 100
            )
        {
            
            this.leaf_kill_distance = leaf_kill_distance;
            this.leaf_influence_radius = leaf_influence_radius;
            this.influence_points = nb_points;
            this.root = root;
            this.nodes.Add(this.root_node);

            this.vol = vol;
        }

        public void start(){
            this.leaves.AddRange(this.PlaceLeaves(this.influence_points));
            this.GenerateRoot();
        }
        

        public void Generate(int max_iterations, int new_leaves_per_iteration = 0)
        {
            this.start();
            while(!this.done && this.steps < max_iterations) {
                this.leaves.AddRange(this.PlaceLeaves(new_leaves_per_iteration));
                this.Grow();
            }
            this.NormalizeThickness();
        }


        // place x number of points around the center
        private List<Leaf> PlaceLeaves(int nbPoints) {

            List<Leaf> leaves = new List<Leaf>();

            Vector3[] points = this.PlacePoints(nbPoints);

            for (int i = 0; i < points.Length; i++) {
                // random between 0 and 1 and test if greater than 0.5
                // bool repulse = Random.value > 0.75f;
                Vector3 position = points[i];
                Leaf leaf = new Leaf(i, position, this.leaf_kill_distance, this.leaf_influence_radius);
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sphere.name = "Leaf " + leaf.id;
                sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                sphere.transform.position = leaf.position;
                sphere.transform.parent = this.root.transform;
                leaf.gameObject = sphere;
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
            clear_colors();
            this.AttractNodes();
            List<Leaf> dropped_leaves = this.DropLeaves();
            List<Node> new_nodes = this.GrowNodes();
            this.steps++;
            return (dropped_leaves, new_nodes);
        }

        public void Attract(){
            this.AttractNodes();
        }

        public List<Node> Grow2(int new_leaves_per_iteration = 0){
            this.DropLeaves();
            List<Node> new_nodes = this.GrowNodes();
            this.steps++;
            clear_colors();
            this.leaves.AddRange(this.PlaceLeaves(new_leaves_per_iteration));
            return new_nodes;
        }

        public void Link(){
            LinkNodes();
        }


        // Pour chaque feuille, on cherche le noeud le plus proche. 
        // Si le noeud est dans le rayon d'influence, on change sa direction vers la feuille.
        public void AttractNodes()
        {
            Leaf leaf;
            Node closest = null;
            List<Node> influenced = new List<Node>();

            for(int i = 0; i < this.leaves.Count; i++)
            {
                leaf = this.leaves[i];
                closest = leaf.FindClosestNode(this.nodes);
                if (closest != null) {
                    influenced.Add(closest);
                    if(leaf.inversed)
                        closest.direction -= this.GetDirection(closest.position, leaf.position);
                    else
                        closest.direction += this.GetDirection(closest.position, leaf.position);
                    closest.influences.Add(leaf);
                    closest.isInfluenced = true;
                }
            }
            SetColors(influenced);
        }

        private void SetColors(List<Node> influenced){
            foreach(Node node in influenced){
                int random = Random.Range(0, this.leaves.Count);
                Color color = GenerateColor(random, this.leaves.Count);
                Color beforeColor = node.gameObject.GetComponent<Renderer>().material.GetColor("_Color");
                if(beforeColor != Color.black) node.gameObject.GetComponent<Renderer>().material.SetColor("_Color", color);
                foreach(Leaf leaf in node.influences){
                    if(!leaf.reached) leaf.gameObject.GetComponent<Renderer>().material.SetColor("_Color", color);
                }
            }
        }

        private Color GenerateColor(int i, int nb){
            float r = (float)i / (float)nb;
            float g = 1 - r;
            float b = Random.Range(0.0f, 1.0f);
            return new Color(r, g, b);
        }
        

        private void clear_colors(){
            foreach(Node node in this.nodes){
                node.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            }
            foreach(Leaf leaf in this.leaves){
                leaf.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            }
        }

        // Pour chaque noeud influencé, on crée une nouvelle branche positionnée dans la direction du noeud. 
        // Le noeud fils hérite de la direction du noeud père.
        public List<Node> GrowNodes()
        {
            List<Node> newNodes = new List<Node>();
            Node node, newNode;
            for(int i = 0; i < this.nodes.Count; i++)
            {
                node = this.nodes[i];
                if(node.isInfluenced) {
                    node.direction /= node.influences.Count + 1;
                    node.direction.Normalize();
                    node.influences = new List<Leaf>();
                    node.isInfluenced = false;
                    
                    newNode = node.CreateNext();
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
        public List<Leaf> DropLeaves() {
            List<Leaf> to_remove = new List<Leaf>();

            foreach (Leaf leaf in this.leaves) {
                if (leaf.reached) {
                    to_remove.Add(leaf);
                }
            }

            foreach (Leaf leaf in to_remove) {
                UnityEngine.Object.DestroyImmediate(leaf.gameObject);
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
                root = root.CreateNext();
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
            int MAX_ITER = 200;

            points.AddRange(this.PlacePointsInMesh(nb_points));

            while(points.Count < nb_points && MAX_ITER-- > 0)
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

        private bool CheckSpacing(Vector3 point, Vector3[] points, int nb_points)
        {
            float minDist = float.MaxValue;
            float dist;
            for (int i = 0; i < nb_points; i++)
            {
                dist = Vector3.Distance(point, points[i]);
                if (dist < minDist)
                {
                    minDist = dist;
                }
            }
            return minDist >= 2;
        }


        private Vector3[] TestPoints(Vector3[] points)
        {

            List<Vector3> newPoints = new List<Vector3>();

            foreach(Vector3 p in points)
            {
                Vector3 depart = p;

                Vector3 direction = this.GetDirection(depart, this.vol.center);
                RaycastHit[] hits = Physics.RaycastAll(depart, direction, 100.0f);
                
                int collisions = hits.Length;

                if(collisions == 0)
                {
                    newPoints.Add(p);
                }
            }

            if(newPoints.Count > 0)
                return newPoints.ToArray();
            else 
                return points;
        }

        public void LinkNodes() 
        {
            List<Node> nodes = this.nodes.FindAll(node => node.parent != null && !node.linked);
            Debug.Log("Linking " + nodes.Count + " nodes");
            foreach(Node node in nodes){
                Debug.Log("Node(" + node.id  + ")");
                if(node.parent != null){
                    GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    capsule.name = "Node(" + node.parent.id + ", " + node.id + ")";
                    capsule.transform.position = (node.position + node.parent.position) / 2f;
                    capsule.transform.parent = this.root.transform;
                    capsule.transform.LookAt(node.position);
                    capsule.transform.Rotate(Vector3.right, 90);
                    node.linked = true;
                }
            }
        }

        public List<Node> GetNodes() {
            return this.nodes;
        }

        public List<Leaf> GetLeaves() {
            return this.leaves;
        }



        public Color generate_color(int index, int total_colors){
            float hue = (index / total_colors) * 360;  // Calculate the hue value based on the index
            float saturation = 0.8f;  // Set a constant saturation value (0.0 to 1.0)
            float lightness = 0.6f;  // Set a constant lightness value (0.0 to 1.0)
            
            // Convert HSL values to RGB values
            Color rgb = hsl_to_rgb(hue, saturation, lightness);
            return rgb;
        }


        // Helper function to convert HSL to RGB
        public Color hsl_to_rgb(float h, float s, float l){
            h /= 360;
            float q = l + s - l * s;
            float p = 2 * l - q;
            float r = hue_to_rgb(p, q, h + 1 / 3);
            float g = hue_to_rgb(p, q, h);
            float b = hue_to_rgb(p, q, h - 1 / 3);
            return new Color(r, g, b);
        }

        private float hue_to_rgb(float p, float q, float t){
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1 / 6) return p + (q - p) * 6 * t;
            if (t < 1 / 2) return q;
            if (t < 2 / 3) return p + (q - p) * (2 / 3 - t) * 6;
            return p;
        }

    }
}
