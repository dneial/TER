
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

    public void addGameObject() {
        this.gameobject = GameObject.CreatePrimitive(PrimitiveType.Cube);
    }

    // method to set the radius of the branch in x and y axis
    public void setRadius(float radius){
        this.gameobject.transform.localScale = new Vector3(radius, radius, this.gameobject.transform.localScale.z);
    }
    public void setHeading(float heading){
        this.gameobject.transform.rotation = Quaternion.Euler(heading, 0, 0);
    }
    public void setPitch(float pitch){
        this.gameobject.transform.rotation = Quaternion.Euler(0, pitch, 0);
    }
    public void setRoll(float roll){
        this.gameobject.transform.rotation = Quaternion.Euler(0, 0, roll);
    }
    public void setStep(float step){
        this.gameobject.transform.position = this.gameobject.transform.position + this.gameobject.transform.forward * step;
    }

}