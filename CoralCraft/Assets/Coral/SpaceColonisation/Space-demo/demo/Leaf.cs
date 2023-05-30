using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
namespace demo {

    public class Leaf {
        public long id { get; }
        public Vector3 position { get; }
        public float kill_distance { get; set; }
        public float influence_radius {get; set; }
        public bool inversed { get; set; } = false;
        public bool reached { get; set; } = false;
        public GameObject gameObject { get; set; }

        public Leaf(long id, Vector3 position, float kill_distance = 1f, float influence_radius = 9f) {
            this.id = id;
            this.position = position;
            this.kill_distance = kill_distance;
            this.influence_radius = influence_radius;
        }

        public Node FindClosestNode(List<Node> nodes) {
            Node closest = null;
            float closestDistance = float.MaxValue;
            foreach (Node node in nodes) {
                float distance = Vector3.Distance(node.position, this.position);
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
}
