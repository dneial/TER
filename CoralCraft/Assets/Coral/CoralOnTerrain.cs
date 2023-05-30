using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class CoralOnTerrain : EditorWindow
{
    [MenuItem("Coral/Generation")]
    public static void showWindow() {
        EditorWindow window = GetWindow(typeof(CoralOnTerrain));
        window.minSize = new Vector2(325, 400);
        window.maxSize = new Vector2(325, 400);
        window.Show();
    }

    static int nbCoraux = 1;
    static AnimationCurve probaHeight;
    static AnimationCurve thickness = AnimationCurve.EaseInOut(0,0.35f,1,1);
    static GameObject prefab;
    static float height = 10f;
    static int max_iterations = 1000;
    static float leaf_influence_radius = 1f;
    static float leaf_kill_distance = 1f;
    static int influence_points = 100;

    private Terrain terrain;
    private TerrainData td;

    void OnGUI() {
        terrain = Terrain.activeTerrain;

        if(terrain == null){
            td = AssetDatabase.LoadAssetAtPath("Assets/Terrain/Pre-Config.asset", typeof(TerrainData)) as TerrainData;
            terrain = Terrain.CreateTerrainGameObject(td).GetComponent<Terrain>();

        } else {
            td = terrain.GetComponent<Terrain>().terrainData;
        }


        probaHeight = AnimationCurve.EaseInOut(td.bounds.max.y, 0.1f, td.bounds.min.y, 1);
        probaHeight = EditorGUILayout.CurveField("Apparition", probaHeight);

        GUILayout.Label("Nombre de generation");
        nbCoraux = EditorGUILayout.IntSlider(nbCoraux, 1, 1000);

        leaf_influence_radius = Random.Range(10, 70);

        leaf_kill_distance = Random.Range(1, 7);

        influence_points = Random.Range(50, 400);

        height = Random.Range(1, 10);

        max_iterations = Random.Range(100, 850);

        thickness.AddKey(0.4f, 0.9f);
        thickness.AddKey(0.3f, 0.8f);
        thickness.AddKey(0.95f, 1);
        thickness.SmoothTangents(3, -1);

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
        while(cpt < nbCoraux){
            float x = Random.Range(posTerrain.x, posTerrain.x+td.size.x);
            float z = Random.Range(posTerrain.z, posTerrain.z+td.size.z);
            float y = Terrain.activeTerrain.SampleHeight(new Vector3(x,0,z)) + Terrain.activeTerrain.transform.position.y;
            if(Random.Range(0f, 1f) < probaHeight.Evaluate(y)) {
                GameObject coral = CreateCoral();
                if (coral == null)
                {
                    Debug.Log("No branches to combine");
                }
                else
                {
                    MeshCombiner combiner = new MeshCombiner(coral);
                    combiner.combineMeshes();
                    combiner.getCombinedMesh().transform.parent = gen.transform;
                    combiner.getCombinedMesh().transform.localPosition = new Vector3(x, y, z);
                    cpt++;
                }
            }
        }
    }

    void RandomGeneration(){
        Terrain terrain = Terrain.activeTerrain;
        TerrainData td = terrain.GetComponent<Terrain>().terrainData;
        Vector3 posTerrain = terrain.transform.localPosition;

        GameObject gen = new GameObject();

        int cpt = 0;
        while(cpt < nbCoraux){
            float x = Random.Range(posTerrain.x, posTerrain.x+td.size.x);
            float z = Random.Range(posTerrain.z, posTerrain.z+td.size.z);
            float y = Terrain.activeTerrain.SampleHeight(new Vector3(x,0,z)) + Terrain.activeTerrain.transform.position.y;
            GameObject coral = CreateCoral();
            if (coral == null)
            {
                Debug.Log("No branches to combine");
            }
            else
            {
                MeshCombiner combiner = new MeshCombiner(coral);
                combiner.combineMeshes();
                combiner.getCombinedMesh().transform.parent = gen.transform;
                combiner.getCombinedMesh().transform.localPosition = new Vector3(x, y, z);
                cpt++;
            }
        }
    }

    GameObject CreateCoral(){
        if(prefab == null)
            {
                prefab = AssetDatabase.LoadAssetAtPath("Assets/Coral/SpaceColonisation/Blender_msh/AsimBox.fbx", typeof(GameObject)) as GameObject;
            }


            if((int)Random.Range(0,2) == 0){
                GameObject go = Instantiate(prefab, new Vector3(0, height, 0), Quaternion.identity);

                if(go.GetComponent<MeshCollider>() == null)
                {
                    // Debug.Log("No mesh collider found, adding one");
                    go.AddComponent<MeshCollider>();
                }

                Bounds bounds = go.GetComponent<MeshCollider>().bounds;
                
                SpaceColonization generator = new SpaceColonization(bounds, leaf_kill_distance, leaf_influence_radius, influence_points);
                SpaceColonizationView view = new SpaceColonizationView();

                DestroyImmediate(go);

                generator.Generate(max_iterations);
                view.update(generator.GetNodes(), thickness);

                return view.GetRoot();

            } else {
                GameObject parent = new GameObject();
                
                List<string> listpath = new List<string>();
                listpath.Add("/Coral/L-System/Grammars/lophelia.lsys2");
                listpath.Add("/Coral/L-System/Grammars/UASG.lsys2");
                listpath.Add("/Coral/L-System/Grammars/custom.lsys");
                
                int grammar = (int)Random.Range(0, listpath.Count);
                string path = listpath[grammar];
                
                Lsystem lsystem = LsystemInterpretor.ParseFile(Application.dataPath + path);
                
                lsystem.Generate((int)Random.Range(3, 5));
                if (path.EndsWith(".lsys"))
                {
                    //traduction de la gammaire lsystemV1 en lsystemV2
                    lsystem.trad(0.75f, 2f, 25f);
                }

                LSystemGen generator = new LSystemGen(lsystem, parent);
                generator.ParseAndPlace(lsystem.current, true);
                if(parent.transform.GetChild(0).position.y > parent.transform.GetChild(1).position.y){
                    parent.transform.Rotate(Vector3.right, 180);
                }
                return parent;
            }     
    }
}
