using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class SpaceColonizationGen : MonoBehaviour
{

    [Range(1f, 10f)]
    public float leaf_kill_distance = 1f;

    [Range(1f, 100f)]
    public float leaf_influence_radius = 9f;

    [Range(50, 1000)]
    public int influence_points = 100;

    
    SpaceColonization generator;
    SpaceColonizationView view;

    public void Start()
    {
        GameObject vol = AssetDatabase.LoadAssetAtPath("Assets/Blender_msh/Vol.prefab", typeof(GameObject)) as GameObject;
        
        vol.transform.localScale = new Vector3(100, 100, 100);
        Mesh mesh = vol.GetComponent<MeshFilter>().sharedMesh;

        Debug.Log("mesh size " + mesh.bounds.size);

        this.generator = new SpaceColonization(this.leaf_kill_distance, 
                                               this.leaf_influence_radius, 
                                               this.influence_points,
                                               mesh);

        this.view = new SpaceColonizationView();

        this.generator.start();
        this.view.update(this.generator.GetLeaves());
        this.view.update(this.generator.GetNodes());
    }


    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            while(!generator.done) {
                this.GenerateAndDrop();
            }

            this.view.update(this.generator.GetNodes());

            Debug.Log("Done @ " + generator.steps + " steps");
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Generating...");
            if(!generator.done) {
                this.GenerateAndShow();
            }
            else Debug.Log("Done @ " + generator.steps + " steps");
        }   
    }


    public void GenerateAndShow()
    {
        (List<Leaf>, List<Node>) gen = this.generator.Grow();
        this.view.DropLeaves(gen.Item1);
        this.view.update(gen.Item2);
    }

    private void GenerateAndDrop()
    {
        (List<Leaf>, List<Node>) gen = this.generator.Grow();
        this.view.DropLeaves(gen.Item1);
        //this.view.update(this.generator.GetNodes());
    }
}