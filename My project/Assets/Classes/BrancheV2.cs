
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrancheV2 
{
    public int id{ get; }
    public Vector3 position{ get; set; }
    public BrancheV2 parent { get; set; }
    public GameObject gameobject { get; set; }
    public BranchState branchState { get; set; }


    public BrancheV2(int id, BrancheV2 parent) {
        this.id = id;
        this.parent = parent;
        this.position = parent.getPosition();
        this.branchState = new BranchState();
    }

    // constructor for the first branch
    public BrancheV2(int id, Vector3 position) {
        this.id = id;
        this.parent = null;
        this.position = position;
        this.branchState = new BranchState();
    }

    public BrancheV2(int id)
    {
        //Fausse branche pour marquer un branchement
        this.id = -1;
        this.parent = null;
        this.position = Vector3.zero;
        this.branchState = null;
    }

    public BrancheV2()
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
        this.position = this.gameobject.transform.position = position;
    }

    public Vector3 getPosition() {
        return this.gameobject.transform.position;
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