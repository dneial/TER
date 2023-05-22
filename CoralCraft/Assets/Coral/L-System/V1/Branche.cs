
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branche : INode
{
    public int id{ get; }
    public Vector3 position{ get; set; }
    public Vector3 rotation { get; set; }
    public INode parent { get; set; }
    public GameObject gameobject { get; set; }
    public BranchState branchState { get; set; }


    public Branche(int id, INode parent) {
        this.id = id;
        this.parent = parent;
        this.position = parent.position;
        this.branchState = new BranchState();
    }

    // constructor for the first branch
    public Branche(int id, Vector3 position) {
        this.id = id;
        this.parent = null;
        this.position = position;
        this.branchState = new BranchState();
    }

    public Branche(int id)
    {
        //Fausse branche pour marquer un branchement
        this.id = -1;
        this.parent = null;
        this.position = Vector3.zero;
        this.branchState = null;
    }

    public Branche()
    {
        this.id = 0;
        this.parent = null;
        this.position = new Vector3(0, 0, 0);
        this.branchState = new BranchState();
    }

    public void addGameObject(GameObject p) {
        this.gameobject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        if (this.parent != null) 
        {
            this.gameobject.name = "edge (" + this.parent.id + " " + this.id + ")";
        }
        else   this.gameobject.name = "edge (" + 0 + " " + this.id + ")";

        this.gameobject.transform.SetParent(p.transform);
    }
    
        
    
    public void setState(BranchState state) {
        this.branchState = state;
    }

    public BranchState getState() {
       return this.branchState;
    }

    public void setPosition(Vector3 position) {
        this.position = position;
    }

    public Vector3 getPosition() {
        return this.position;
    }

    public void setGPosition(Vector3 position) {
        this.gameobject.transform.position = position;
    }

    public Vector3 getGPosition() {
        return this.gameobject.transform.position;
    }

    public void setRotation(Vector3 rotation) {
        this.rotation = rotation;
    }

    public Vector3 getRotation() {
        return this.rotation;
    }

    public void setGRotation(Vector3 rotation) {
        this.gameobject.transform.rotation = Quaternion.Euler(rotation);
    }

    public Vector3 getGRotation() {
        return this.gameobject.transform.rotation.eulerAngles;
    }

    // to string method
    public override string ToString()
    {
        if (this.parent == null) 
        {
            return "branche: [" + this.id + "] (" + 0 + ", " +this.id +")" +
                    "position: ( "+ this.gameobject.transform.position.x +" ,"+ this.gameobject.transform.position.y +" ,"+ this.gameobject.transform.position.z +")" ;
        }
        return "branche: [" + this.id + "] (" + this.parent.id + ", " +this.id +")" +
                "position:  ( "+ this.gameobject.transform.position.x +" ,"+ this.gameobject.transform.position.y +" ,"+ this.gameobject.transform.position.z +")" ;
    }
}