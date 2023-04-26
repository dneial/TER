using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class TestTerrain : EditorWindow
{
    [MenuItem("Terrain/Generation")]
    public static void showWindow() {
        EditorWindow window = GetWindow(typeof(TestTerrain));
        window.Show();
    }

    static int nbCoraux = 1;
    static AnimationCurve probaHeight;

    void OnGUI() {
        Terrain terrain = Terrain.activeTerrain;
        TerrainData td = terrain.GetComponent<Terrain>().terrainData;

        probaHeight = AnimationCurve.EaseInOut(td.bounds.min.y, 1, td.bounds.max.y, 0.1f);
        probaHeight = EditorGUILayout.CurveField("Apparition", probaHeight);

        GUILayout.Label("Nombre de generation");
        nbCoraux = EditorGUILayout.IntSlider(nbCoraux, 50, 1000);

        if(GUILayout.Button("RandomGeneration")) {
            RandomGeneration();
        }
        if(GUILayout.Button("ProbaGeneration")) {
            ProbaGeneration();
        }
    }

    void ProbaGeneration(){
        Terrain terrain = Terrain.activeTerrain;
        TerrainData td = terrain.GetComponent<Terrain>().terrainData;
        Vector3 posTerrain = terrain.transform.localPosition;

        GameObject gen = new GameObject();

        int cpt = 0;
        while(cpt <= nbCoraux){
            float x = Random.Range(posTerrain.x, posTerrain.x+td.size.x);
            float z = Random.Range(posTerrain.z, posTerrain.z+td.size.z);
            float y = Terrain.activeTerrain.SampleHeight(new Vector3(x,0,z)) + Terrain.activeTerrain.transform.position.y;
            if(Random.Range(0f, 1f) < probaHeight.Evaluate(y)) {
                cpt++;
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                sphere.transform.localPosition = new Vector3(x, y, z);
                sphere.transform.parent = gen.transform;
            }
        }
    }

    void RandomGeneration(){
        Terrain terrain = Terrain.activeTerrain;
        TerrainData td = terrain.GetComponent<Terrain>().terrainData;
        Vector3 posTerrain = terrain.transform.localPosition;

        GameObject gen = new GameObject();

        int cpt = 0;
        while(cpt <= nbCoraux){
            float x = Random.Range(posTerrain.x, posTerrain.x+td.size.x);
            float z = Random.Range(posTerrain.z, posTerrain.z+td.size.z);
            float y = Terrain.activeTerrain.SampleHeight(new Vector3(x,0,z)) + Terrain.activeTerrain.transform.position.y;
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
            sphere.transform.localPosition = new Vector3(x, y, z);
            sphere.transform.parent = gen.transform;
            cpt++;
        }
    }
}
