using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    Lsystem lsystem;
    List<Vector3> points = new List<Vector3>();
    

    // Start is called before the first frame update

    void Start()
    {
        Dictionary<char, string> rules = new Dictionary<char, string>();
        rules.Add('A', "B[-A][+A]B");
        rules.Add('B', "BB");
        lsystem = new Lsystem("A", rules);
        lsystem.Generate(2);
        lsystem.Generate(2);
        GetPoints(lsystem.current);
        PlacePoints();

        Debug.Log(lsystem.current);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetPoints(string current)
    {
        Vector3 pos = Vector3.zero;
        Vector3 dir = Vector3.up;
        float angle = 25f;
        float length = 1f;

        Stack<Vector3> posStack = new Stack<Vector3>();
        Stack<Vector3> dirStack = new Stack<Vector3>();
        points.Add(pos);

        for (int i = 1; i < current.Length; i++)
        {
            char c = current[i];
            if (c == 'A' || c == 'B')
            {
                Vector3 next = pos + dir * length;
                points.Add(next);
                pos = next;
            }
            else if (c == '+')
            {
                dir = Quaternion.AngleAxis(angle, Vector3.forward) * dir;
            }
            else if (c == '-')
            {
                dir = Quaternion.AngleAxis(-angle, Vector3.forward) * dir;
            }
            else if (c == '[')
            {
                posStack.Push(pos);
                dirStack.Push(dir);
            }
            else if (c == ']')
            {
                dir = dirStack.Pop();
                pos = posStack.Pop();
            }
        }
    }

    public void PrintPoints()
    {
        for (int i = 0; i < points.Count; i += 1)
        {
            Debug.Log(points[i]);
        }
    }

    public void PlacePoints()
    {
        for (int i = 0; i < points.Count; i += 1)
        {
            Vector3 start = points[i];
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            line.transform.position = start;
            line.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }
}
