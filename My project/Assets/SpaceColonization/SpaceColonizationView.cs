using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
public class SpaceColonizationView {

    public void PlaceLeaves(List<Leaf> leaves)
    {

    }



    public void LinkNodes(List<Node> nodes) 
    {
        foreach(Node node in nodes){
            if(node.parent != null){
                GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                cylinder.name = "Node(" + node.parent.id + ", " + node.id + ")";
                cylinder.transform.position = (node.position + node.parent.position) / 2f;
                cylinder.transform.localScale = new Vector3(1f, Vector3.Distance(node.position, node.parent.position), 1f);
                cylinder.transform.parent = this.root.transform;
                cylinder.transform.LookAt(node.position);
                cylinder.transform.Rotate(Vector3.right, 90);
                node.cylindre = cylinder;
                this.node_links.Add(cylinder);
            }
        }
    }
}