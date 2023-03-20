using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptGen1 : MonoBehaviour
{
    Lsystem lsystem;

    List<Branche> points = new List<Branche>();

    LsystemInterpretor lSysInterp;

    GameObject parent;
    public int nbIteration = 2;
    public float angle = 25f;
    public float noise = 5f;
    public string grammarFile = "Assets/Grammar/First_coral.lsys";

    // Start is called before the first frame update
    void Start()
    {

        parent = gameObject; 

        // Dictionary<char, string> rules = new Dictionary<char, string>();
        // rules.Add('A', "B[-A][+A]B");
        // rules.Add('B', "BB");
        // string[] variables = {"A", "B"};
        // string[] constantes = {"+", "-", "[", "]"};

        // lsystem = new Lsystem(new List<string>(variables), new List<string>(constantes), "A", rules);

        lsystem = new Lsystem(new List<string>(), new List<string>(), "", new Dictionary<char, string>());
        LsystemInterpretor lSysInterp = new LsystemInterpretor(grammarFile, lsystem, nbIteration);
        
        lSysInterp.interpret();

        lsystem.Generate(nbIteration);

        Debug.Log(lsystem.ToString());


        GetPoints(lsystem.current);
        PlacePoints();

        Debug.Log(lsystem.current);

    }

    // Update is called once per frame
    // void Update()
    // {

    // }

    public void GetPoints(string current)
    {
        Vector3 pos = Vector3.zero;
        Vector3 dir = Vector3.up;
        float angle = lsystem.angle;
        float length = 1f;

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
                Vector3 next = pos + dir * length;
                Branche branche = new Branche(++cpt, next);
                racine.addChild(branche);
                points.Add(branche);
                racine = branche;
                pos = next;
            }
            else if (c == '+')
            {
                dir = Quaternion.AngleAxis(addNoise(angle, noise), Vector3.forward) * dir;
            }
            else if (c == '-')
            {
                dir = Quaternion.AngleAxis(-addNoise(angle, noise), Vector3.forward) * dir;
            }
            else if (c == '[')
            {
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


                line.name = "edge (" + branche.getId() + " " + fils.getId() + ")";
                line.transform.SetParent(parent.transform);

                line.transform.position = (start + end) / 2;
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


    //add noise to the angle
    public float addNoise(float angle, float noiseAmmount)
    {
        float noise = UnityEngine.Random.Range(-noiseAmmount, noiseAmmount);
        return angle + noise;
    }
    
}