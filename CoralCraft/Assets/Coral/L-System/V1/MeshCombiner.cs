using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner
{
    private List<MeshFilter> sourceMeshFilters;
    private GameObject combinedMeshGameObject;
    private GameObject parentMesh;

    private Material[] mats;

    public MeshCombiner(GameObject parentMesh, string name = "CombinedMesh")
    {
        this.parentMesh = parentMesh;

        //create a new gameobject for the combined mesh
        this.combinedMeshGameObject = new GameObject(name);
        
        //set it as the target mesh
        // this.targetMeshFilter = combinedMeshGameObject.AddComponent<MeshFilter>();

        //get all the children mesh filters
        this.sourceMeshFilters = new List<MeshFilter>();
        foreach (Transform child in parentMesh.transform)
        {
            var meshFilter = child.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                sourceMeshFilters.Add(meshFilter);
            }
            //if child has children
            if (child.childCount > 0)
            {
                foreach (Transform grandChild in child.transform)
                {
                    var grandChildMeshFilter = grandChild.GetComponent<MeshFilter>();
                    if (grandChildMeshFilter != null)
                    {
                        sourceMeshFilters.Add(grandChildMeshFilter);
                    }
                }
            }
        }

        this.mats = Resources.LoadAll<Material>("Materials/"); 
    }

    public GameObject getCombinedMesh()
    {
        return this.combinedMeshGameObject;
    }

    public void combineMeshes()
    {
        var combine = new CombineInstance[sourceMeshFilters.Count];

        for (var i = 0; i < sourceMeshFilters.Count; i++)
        {
            combine[i].mesh = sourceMeshFilters[i].sharedMesh;
            combine[i].transform = sourceMeshFilters[i].transform.localToWorldMatrix;
        }

        this.combinedMeshGameObject.AddComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        this.combinedMeshGameObject.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

        //Debug.Log("Meshes combined");
        //TEXTURES ICI

        //attribuer aléatoirement un matériau de la liste
        //Debug.Log("mats = " + this.mats.Length);
        this.combinedMeshGameObject.AddComponent<MeshRenderer>().material = this.mats[Random.Range(0, mats.Length-1  )];

        // this.combinedMeshGameObject.AddComponent<MeshRenderer>().material = sourceMeshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;
        //set the color to light pink
        //this.combinedMeshGameObject.GetComponent<MeshRenderer>().material.color = new Color(1f, 0.5f, 0.5f, 1f);

        //delete parentmesh
        Object.DestroyImmediate(parentMesh);

        
    }
}