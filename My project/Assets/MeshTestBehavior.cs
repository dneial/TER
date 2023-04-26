using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class MeshTestBehavior : MonoBehaviour
{
    public GameObject prefab;
    
    [Range(10, 100)]
    public int points = 10;

    private Mesh mesh;
    private GameObject root;

    void Start()
    {
        this.root =  Instantiate(prefab,new Vector3(0, 10, 0), Quaternion.identity);
        
        this.mesh = this.root.GetComponent<MeshFilter>().sharedMesh;
        Debug.Log("mesh center : " + this.mesh.bounds.center);

        this.PlacePointsInMesh(points);
    }

    private Vector3[] PlacePointsInMesh(int nb_points)
    {

        Mesh mesh = this.mesh;
        Vector3[] points = new Vector3[nb_points];



        Vector3 min = mesh.bounds.min;
        Vector3 max = mesh.bounds.max;

        Debug.Log("min: " + min);
        Debug.Log("max: " + max);


        for (int i = 0; i < nb_points; i++)
        {
            Vector3 randomPoint = this.root.transform.position;
            randomPoint.x += Random.Range(min.x, max.x);
            randomPoint.y += Random.Range(min.y, max.y);
            randomPoint.z += Random.Range(min.z, max.z);

            points[i] = randomPoint;
        }


        this.DrawPoints(points);
        points = this.TestPoints(points);

        return points;
    }


    private Vector3[] TestPoints(Vector3[] points)
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;


        Vector3[] newPoints = new Vector3[points.Length];

        foreach(Vector3 p in points)
        {
            Vector3 depart = p;
            Debug.Log("depart: " + depart);
            RaycastHit[] hits = Physics.RaycastAll(depart, Vector3.up, 100.0f);
            RaycastHit hit;

            int collisions = 0;
            
            if(hits.Length == 1) 
            {
                hit = hits[0];
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.name = "hit";
                go.transform.position = hit.point;
                go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());


                if(Physics.Raycast(hit.point, Vector3.up, 100.0f, layerMask))
                {
                    Debug.Log("2nd hit");
                }
                else
                {
                    Debug.Log("no 2nd hit");
                }
            }
            else Debug.Log("no hit");
        

            if(collisions%2 != 0)
            {
                Debug.Log("collisions: " + collisions);
            }
        }

        return points;
    }


    void DrawPoints(Vector3[] points)
    {
        foreach(Vector3 p in points)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Leaf";
            sphere.transform.localScale = new Vector3(1f, 1f, 1f);
            sphere.transform.position = p;
            sphere.transform.parent = this.root.transform;
            UnityEngine.Object.DestroyImmediate(sphere.GetComponent<Collider>());
        }
    }
}