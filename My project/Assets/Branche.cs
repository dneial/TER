
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Branche 
{
    private long id;
    private Vector3 position;
    private List<Branche> children;
    private GameObject gameobject;

    public Branche(long id, Vector3 position) {
        this.id = id;
        this.position = position;
        this.children = new List<Branche>();
    }

    public void addChild(Branche child) {
        this.children.Add(child);
    }

    public long  getId() {
        return this.id;
    }

    public List<Branche> getChildren() {
        return this.children;
    }

    public Vector3 getPosition() {
        return this.position;
    }

    public void setPosition(Vector3 position) {
        this.position = position;
    }

}