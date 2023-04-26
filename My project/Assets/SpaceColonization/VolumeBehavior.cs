using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class VolumeBehavior : MonoBehaviour 
{

    private Collider volume;

    void Start()
    {
        this.volume = this.GetComponent<Collider>();
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with " + collision.gameObject.name);
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
    }
}