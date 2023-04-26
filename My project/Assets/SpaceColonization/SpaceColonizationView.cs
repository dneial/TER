using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class SpaceColonizationView {

    private Dictionary<Leaf, GameObject> leaves = new Dictionary<Leaf, GameObject>();

    private GameObject root = new GameObject("Root");

    public SpaceColonizationView()
    {
        this.root.transform.position = Vector3.zero;
    }

    public void update(List<Leaf> leaves)
    {
        this.PlaceLeaves(leaves);
    }

    public void update(List<Node> nodes, AnimationCurve thickness)
    {
        this.LinkNodes(nodes, thickness);
    }


    public void PlaceLeaves(List<Leaf> leaves)
    {
        foreach(Leaf leaf in leaves){
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Leaf " + leaf.id;
            sphere.transform.localScale = new Vector3(1f, 1f, 1f);
            sphere.transform.position = leaf.position;
            sphere.transform.parent = this.root.transform;
            this.leaves.Add(leaf, sphere);
        }
    }


    public void LinkNodes(List<Node> nodes, AnimationCurve thickness) 
    {
        foreach(Node node in nodes){
            if(node.parent != null){
                GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                capsule.name = "Node(" + node.parent.id + ", " + node.id + ")";
                capsule.transform.position = (node.position + node.parent.position) / 2f;
                Debug.Log(node.thickness);
                float actualThickness = thickness.Evaluate(node.thickness);
                capsule.transform.localScale = new Vector3(actualThickness, (actualThickness+Vector3.Distance(node.position, node.parent.position))/2, actualThickness);
                if(node.parent != null){
                    MajVerticesCapsule(capsule, actualThickness,thickness.Evaluate(node.parent.thickness));
                } else {
                    MajVerticesCapsule(capsule, actualThickness,1);
                }
                capsule.transform.parent = this.root.transform;
                capsule.transform.LookAt(node.position);
                capsule.transform.Rotate(Vector3.right, 90);
                node.cylindre = capsule;
            }
        }
    }

    private void MajVerticesCapsule(GameObject caps, float topThickness, float botThickness){
        Mesh myMesh = caps.transform.GetComponent<MeshFilter>().mesh;
        var myVertices = new Vector3[myMesh.vertices.Length];
        for (int i = 0; i < myMesh.vertices.Length; i++) {
            if(myMesh.vertices[i].y > 0){
                myVertices[i] = new Vector3(myMesh.vertices[i].x*topThickness, myMesh.vertices[i].y, myMesh.vertices[i].z*topThickness);
            } else {
                myVertices[i] = new Vector3(myMesh.vertices[i].x*botThickness, myMesh.vertices[i].y, myMesh.vertices[i].z*botThickness);
            }
        }
        myMesh.SetVertices(myVertices);
        caps.transform.GetComponent<MeshFilter>().mesh = myMesh;
    }

    public void DropLeaves(List<Leaf> leaves)
    {
        foreach(Leaf leaf in leaves){
            UnityEngine.Object.DestroyImmediate(this.leaves[leaf]);
        }
    }
}