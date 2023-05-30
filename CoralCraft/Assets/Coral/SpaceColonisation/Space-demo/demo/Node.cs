using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
namespace demo {
public class Node {
    
    public Vector3 position { get; set; }
    public Vector3 direction { get; set; }
    public Vector3 originalDirection { get; set; }
    public Node parent { get; set; }

    public float step { get; set; } = 1f;
    public float thickness { get; set; } = 1f;
    public List<Leaf> influences { get; set; } = new List<Leaf>();
    public bool isInfluenced { get; set; } = false;
    public GameObject gameObject { get; set; } = null;
    public int id { get; }

    static int counter = 0;

    public Node(Vector3 position, Vector3 direction , Node parent = null) {
        this.id = counter++;
        this.position = position;
        this.direction = direction;
        this.parent = parent;
        this.originalDirection = direction;
         
        this.gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        this.gameObject.name = "Node " + this.id;
        this.gameObject.transform.position = this.position;
        this.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        this.gameObject.transform.parent = this.parent?.gameObject.transform;
        
    }

    public Node CreateNext() {
        return new Node(this.position + this.direction * this.step, this.direction, this);
    }
}
}