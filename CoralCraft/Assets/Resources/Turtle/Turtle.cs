using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


public class Turtle : MonoBehaviour
{
    public GameObject turtle3D;
    private TurtleState state;
    //contains the turtle's state : position, direction, radius, step, heading, pitch, roll

    private Stack<TurtleState> stack = new Stack<TurtleState>();

    public Lsystem lsystem;

    public string production;
    public int wordPointer = 0;

    public GameObject racine;

    // getters/setters of the turtle's state

    public Vector3 position
    {
        get { return state.position; }
        set { state.position = value; }
    }

    public Vector3 direction
    {
        get { return state.direction; }
        set { state.direction = value; }
    }

    public float heading
    {
        get { return state.heading; }
        set { state.heading = value; }
    }

    public float pitch
    {
        get { return state.pitch; }
        set { state.pitch = value; }
    }

    public float roll
    {
        get { return state.roll; }
        set { state.roll = value; }
    }

    public float radius
    {
        get { return state.radius; }
        set { state.radius = value; }
    }

    public float step
    {
        get { return state.step; }
        set { state.step = value; }
    }

    //other getters/setters

    public void setProduction(string production)
    {
        this.production = production;
    }


    //turtle methods
    public void move(float distance)
    {
        //place a cylinder between the previous position and the new one
        
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = position;
        cylinder.transform.rotation = turtle3D.transform.rotation;
        cylinder.transform.localScale = new Vector3(radius, distance/2, radius);
        cylinder.transform.Translate(0, 0, distance/2);
        cylinder.transform.parent = racine.transform;
        
        turtle3D.transform.Translate(0, 0, distance);
        position += distance * direction;

    }

    public void changeRadius(float radius)
    {
        this.radius = radius;
    }

    public void changeHead(float angle)
    {
        turtle3D.transform.rotation = Quaternion.Euler(0, angle, 0);
        heading += angle;
    }

    public void changePitch(float angle)
    {
        turtle3D.transform.rotation = Quaternion.Euler(angle, 0, 0);
        pitch += angle;
    }

    public void changeRoll(float angle)
    {
        turtle3D.transform.rotation = Quaternion.Euler(0, 0, angle);
        roll -= angle;
    }

    public void push()
    {
        stack.Push(new TurtleState(state));
    }

    public void pop()
    {
        state = stack.Pop();
        turtle3D.transform.position = state.position;
        turtle3D.transform.rotation = Quaternion.Euler(state.heading, state.pitch, state.roll);
    }
    

    //Parsing methods
    public string getNextWord()
    {
        string word = "";
        Debug.Log("inside getNextWord");
        Debug.Log("wordPointer = " + wordPointer);
        Debug.Log("production.Length = " + production.Length);
        Debug.Log("production[wordPointer] = " + production[wordPointer]);
        while (wordPointer < production.Length && production[wordPointer] != ' ')
        {
            word += production[wordPointer];
            wordPointer++;
        }
        wordPointer++;
        return word;
    }

    //interpretation methods
    public void interpret(string word)
    {
        char c = word[0];
        string param = word.Substring(1);
        switch (c)
        {
            case 'F' : 
                interpretF(param);
                break;
            
            case 'R' : 
                interpretR(param);
                break;

            case 'h' :
                interpretHeading(param);
                break;
            case 'p' :
                interpretPitch(param);
                break;
            case 'r' :    
                interpretRoll(param);
                break;

            case '[' : 
                interpretPush(param);
                break;
            case ']' :
                interpretPop(param);
                break;

            default:
                interpretVariable(c, param);
                break;
        }
    }

    // #1: F0.75 F(6,1.5) B [ h-22.5 A ] [ h22.5 A ] B

    public void interpretF(string param)
    {        
        if (param != "")
        {
            move(valueParse(param));
        }
        else
        {
            move(step);
        }

    }

    public void interpretR(string param)
    {        
        if (param != "")
        {
            changeRadius(valueParse(param));
        }
        else
        {
           Debug.Log("Warning : Radius change missing parameters");
        }
    }

    public void interpretHeading(string param)
    {
        if (param != "")
        {
            changeHead(valueParse(param));
        }
        else
        {
            changeHead(heading);
        }
    }

    public void interpretPitch(string param)
    {
        if (param != "")
        {
            changePitch(valueParse(param));
        }
        else
        {
            changePitch(pitch);
        }
    }

    public void interpretRoll(string param)
    {
        if (param != "")
        {
            changeRoll(valueParse(param));
        }
        else
        {
            changeRoll(roll);
        }
    }

    public void interpretPush(string param)
    {
        if (param != "")
        {
           Debug.Log("Warning : push does not take any parameter");
        }
        push();
    }

    public void interpretPop(string param)
    {
        if (param != "")
        {
           Debug.Log("Warning : pop does not take any parameter");
        }
        pop();
    }

    public void interpretVariable(char c, string param)
    {
        Debug.Log("untransformed variable : " + c + " with param : " + param + "");
        // string rule = lsystem.rules[c];
        // string[] words = rule.Split(' ');
        // foreach (string word in words)
        // {
        //     interpret(word);
        // }
    }

    //run methods

    private void Start() {
        Debug.Log("production : "+production);
        
        state = new TurtleState();
        racine = new GameObject("racine");

        turtle3D = this.gameObject;
        turtle3D.transform.position = racine.transform.position = position;
        turtle3D.transform.rotation = racine.transform.rotation = Quaternion.Euler(direction);
        
        wordPointer = 0;
    }

    private void Update() {       
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //camera follow the turtle 
            Camera.main.transform.position = turtle3D.transform.position + new Vector3(0, 0, -15);
           
            if (wordPointer < production.Length)
            {
                string word = getNextWord();
                Debug.Log("word : " + word);
                Debug.Log("wordPointer : " + wordPointer);
                interpret(word);
            }
        }

        //DEBUG

        // fleche du haut : avancer
        // fleche du bas : reculer
        // fleche de gauche : tourner a gauche
        // fleche de droite : tourner a droite
        // q : roll + 
        // d : roll -
        // z : pitch +
        // s : pitch -

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            move(2);
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            move(-2);
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            changeHead(-10);
        }
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            changeHead(10);
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            changeRoll(10);
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            changeRoll(-10);
        }
        if(Input.GetKeyDown(KeyCode.Z))
        {
            changePitch(10);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            changePitch(-10);
        }

    }

    //util methods
    public float valueParse(string s){
        
        //deux types de valeurs possibles :
        // (5,1.0) -> 5 + random(-1.0, 1.0)
        // -40     -> -40

        if(s[0] == '('){
            string[] values = s.Substring(1, s.Length - 2).Split(',');  // {"5", "1.0"}
            float value = float.Parse(values[0].Replace('.', ','));
            float randomizer = float.Parse(values[1].Replace('.', ','));

            value += Random.Range(-randomizer, randomizer);
            return value;
        } else {
            return float.Parse(s.Replace('.', ','));
        }
    }
    
}