using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
namespace demo {
public class SpaceColonizationGen : MonoBehaviour
{

    [Range(1f, 10f)]
    public float leaf_kill_distance = 1f;

    [Range(1f, 100f)]
    public float leaf_influence_radius = 9f;

    [Range(50, 1000)]
    public int influence_points = 100;

    [Range(0, 100)]
    public int new_leaves_per_step = 0;

    public GameObject prefab;

    SpaceColonization generator;
    SpaceColonizationView view;

    private int etape = 0;

    public void Start()
    {

        GameObject go = Instantiate(prefab, new Vector3(0, 10f, 0), Quaternion.identity);


        Bounds bounds = go.GetComponent<MeshCollider>().bounds;

        Vector3 size = go.transform.localScale;
        float scale = Mathf.Sqrt(influence_points) / 10f;


        Debug.Log("scale for " + influence_points + "points : " + scale);
        go.transform.localScale = size * scale;

        bounds.size *= scale;



        this.generator = new SpaceColonization(bounds,
                                               this.gameObject,
                                               this.leaf_kill_distance, 
                                               this.leaf_influence_radius, 
                                               this.influence_points
                                               );



        this.generator.start();

        DestroyImmediate(go.GetComponent<MeshRenderer>());

    }


    public void Update()
    {

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Generating...");
            if(!generator.done) {
                if(etape % 2 == 0)
                    this.generator.Attract();
                else
                    this.generator.Grow2(new_leaves_per_step);


                etape++;
            }
            else Debug.Log("Done @ " + generator.steps + " steps");
        }   
    }


    public void Generate()
    {
        (List<Leaf>, List<Node>) gen = this.generator.Grow();
    }

}
}
