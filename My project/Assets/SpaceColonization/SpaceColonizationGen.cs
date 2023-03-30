using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class SpaceColonizationGen : MonoBehaviour
{

    [Range(1f, 10f)]
    public float leaf_kill_distance = 1f;

    [Range(1f, 10f)]
    public float leaf_influence_radius = 9f;

    [Range(50, 1000)]
    public int influence_points = 100;

    SpaceColonization generator;

    public void Start()
    {
        this.generator = new SpaceColonization(this.leaf_kill_distance, this.leaf_influence_radius, this.influence_points);
    }


    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            while(!generator.done) generator.Generate();
            Debug.Log("Done @ " + generator.steps + " steps");
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(!generator.done) generator.Generate();
            else Debug.Log("Done @ " + generator.steps + " steps");
        }   
    }

}