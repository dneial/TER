using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Node {
    public Vector3 position { get; set; }
    public Vector3 direction { get; set; }
    public Vector3 originalDirection { get; set; }
    public Node parent { get; set; }
    public float step { get; set; } = 1f;
    public float thickness { get; set; } = 1f;
    public int influences { get; set; } = 0;
    public bool isInfluenced { get; set; } = false;
    public GameObject cylindre { get; set; } = null;
    static int counter = 0;
    public int id { get; }

    public Node(Vector3 position, Vector3 direction , Node parent = null) {
        this.id = counter++;
        this.position = position;
        this.direction = direction;
        this.parent = parent;
        this.originalDirection = direction;
    }

    public Node GetNext() {
        return new Node(this.position + this.direction * this.step, this.direction, this);
    }
}