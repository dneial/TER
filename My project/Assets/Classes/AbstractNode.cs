using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class AbstractNode : INode{
    public int id { get; }

    public Vector3 position { get; set; }

    public Vector3 rotation { get; set; }

    public INode parent { get; set; }

    public AbstractNode(int id, INode parent) {
        this.id = id;
        this.parent = parent;
        this.position = parent.position;
    }

    public string toString() {
        return "Node " + this.id 
                + " position: (" + this.position.x + ", " 
                              + this.position.y + ", " 
                              + this.position.z + ") "
                + " parent: " + this.parent.id;
    }

    public void displayNodes(List<AbstractNode> nodes, GameObject root) {
        foreach (AbstractNode node in nodes) {
            if (node.parent != null) {

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Node (" + node.parent.id + ", " + node.id + ")";
            body.transform.position = (node.position + node.parent.position) / 2f;
            body.transform.localScale = new Vector3(1f, (1f + Vector3.Distance(node.position, node.parent.position)) / 2, 1f);
            body.transform.parent = root.transform;
            body.transform.LookAt(node.position);
            body.transform.Rotate(Vector3.right, 90);

            }
        }
    }
}
