using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Leaf {
    public long id { get; }
    public Vector3 position { get; }
    public GameObject gameobject { get; set; }
    public float kill_distance { get; set; } = 1f;
    public float influence_radius {get; set; } = 9f;
    public bool reached { get; set; } = false;

    public Leaf(long id, Vector3 position) {
        this.id = id;
        this.position = position;
    }

    public Node FindClosestNode(List<Node> nodes) {
        Node closest = null;
        float closestDistance = float.MaxValue;
        foreach (Node node in nodes) {
            float distance = Vector3.Distance(node.position, this.position);
            Debug.Log("distance leaf " + this.id + " to node " + node.id + ": " + distance);

            if (distance <= kill_distance) {
                this.reached = true;
                closest = node;
                break;
            }

            else if (distance <= closestDistance && distance <= influence_radius) {

                closest = node;
                closestDistance = distance;
            }
        }

        return closest;
    }
}