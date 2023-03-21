
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branche 
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

    public long  getId() {
        return this.id;
    }

    public Vector3 getPosition() {
        return this.position;
    }

    public List<Branche> getChildren() {
        return this.children;
    }

    public GameObject getGameObject() {
        return this.gameobject;
    }

    
    public void setPosition(Vector3 position) {
        this.position = position;
    }

    public void addChild(Branche child) {
        this.children.Add(child);
    }

    public void setGameObject(GameObject gameobject) {
        this.gameobject = gameobject;
    }

}