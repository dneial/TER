using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    Lsystem lsystem;
    public float lower_angle = 25f;
    public float upper_angle = 45f;
    public float length = 1f;
    public float branch_chance = 0.8f;
    List<Branche> points = new List<Branche>();
    

    // Start is called before the first frame update

    void Start()
    {
        Dictionary<char, string> rules = new Dictionary<char, string>();
        rules.Add('A', "B[-A][+A]B");
        rules.Add('B', "BB");
        string[] variables = {"A", "B"};
        string[] constantes = {"+", "-", "[", "]"};
        lsystem = new Lsystem(new List<string>(variables), new List<string>(constantes), "A", rules);
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

        Stack<Vector3> posStack = new Stack<Vector3>();
        Stack<Vector3> dirStack = new Stack<Vector3>();
        Stack<Branche> racineStack = new Stack<Branche>();

        Branche racine = new Branche(0, pos);
        points.Add(racine);
        racineStack.Push(racine);
    
         // BB[-B[-A][+A]B][+B[-A][+A]B]BB

        for (int i = 1, cpt = 0; i < current.Length; i++)
        {
            char c = current[i];
            if (lsystem.variables.Contains(c.ToString()))
            {
                Vector3 next = pos + dir * this.length;
                Branche branche = new Branche(++cpt, next);
                racine.addChild(branche);
                points.Add(branche);
                racine = branche;
                pos = next;
            }
            else if (c == '+')
            {
                float angle = UnityEngine.Random.Range(lower_angle, upper_angle);
                dir = Quaternion.AngleAxis(angle, Vector3.forward) * dir;
            }
            else if (c == '-')
            {
                float angle = UnityEngine.Random.Range(lower_angle, upper_angle);
                dir = Quaternion.AngleAxis(-angle, Vector3.forward) * dir;
            }
            else if (c == '[')
            {
                // chance of skipping a branch
                if (UnityEngine.Random.value >= this.branch_chance)
                {
                    int j = findClosingBracket(current, i);
                    if (j == -1)
                    {
                        break;
                    }
                    i = j;
                    continue;
                }
                posStack.Push(pos);
                dirStack.Push(dir);
                racineStack.Push(racine);
            }
            else if (c == ']')
            {
                dir = dirStack.Pop();
                pos = posStack.Pop();
                racine = racineStack.Pop();
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
            Branche branche = points[i];
            Vector3 start = branche.getPosition();
            foreach(var fils in branche.getChildren())
            {
                Vector3 end = fils.getPosition();
                GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
                line.transform.parent = this.gameObject.transform;
                line.transform.position = (start + end) / 2;
                //line.transform.rotation = Quaternion.LookRotation(end - start);
                line.transform.localScale = new Vector3(0.1f, 0.1f, Vector3.Distance(start, end));
                line.transform.LookAt(end);
            }
        }
    }

    public void PlaceEdges()
    {
        for (int i = 0; i < points.Count; i += 1)
        {
            Vector3 start = points[i].getPosition();
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            line.transform.position = start;
            line.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Debug.Log("point " + points[i].getId() + points[i].getChildren());
        }
    }

    private int findClosingBracket(string s, int i)
    {
        int cpt = 0;
        for (int j = i; j < s.Length; j++)
        {
            if (s[j] == '[')
            {
                cpt++;
            }
            else if (s[j] == ']')
            {
                cpt--;
            }
            if (cpt == 0)
            {
                return j;
            }
        }
        return -1;
    }
}
