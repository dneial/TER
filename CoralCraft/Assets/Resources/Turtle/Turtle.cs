using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


public class Turtle : MonoBehaviour
{
    private TurtleState state;
    //contains the turtle's state : position, rotation, radius, step, heading, pitch, roll

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

    public Vector3 rotation
    {
        get { return state.rotation; }
        set { state.rotation = value; }
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
        set { state.radius = value;}
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

        GameObject segment = new GameObject();
        
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.rotation = Quaternion.Euler(90, 0, 0);
        cylinder.transform.localScale = new Vector3(radius, distance / 2, radius);
        cylinder.transform.parent = segment.transform;

        //place the new segment between the previous position and the new one
        segment.transform.position = (this.transform.position + transform.forward * distance / 2);
        segment.transform.parent = racine.transform;
        segment.transform.rotation = this.transform.rotation;

        this.transform.position += transform.forward * distance;
        position += transform.forward * distance;

        //place a sphere at the end of the segment
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = this.transform.position;
        sphere.transform.localScale = new Vector3(radius, radius, radius);
        sphere.transform.parent = racine.transform;

    }

    public void changeRadius(float radius)
    {
        this.radius = radius;
    }

    public void changeHead(float angle)
    {
        this.transform.rotation = Quaternion.Euler(0, angle, 0) * this.transform.rotation;
        heading += angle;
        rotation = this.transform.rotation.eulerAngles;
    }

    public void changePitch(float angle)
    {
        this.transform.rotation = Quaternion.Euler(angle, 0, 0) * this.transform.rotation;
        pitch += angle;
        rotation = this.transform.rotation.eulerAngles;
    }

    public void changeRoll(float angle)
    {
        this.transform.rotation = Quaternion.Euler(0, 0, angle) * this.transform.rotation;
        roll += angle;
        rotation = this.transform.rotation.eulerAngles;
    }

    public void push()
    {
        Debug.Log("actual state = " + state);
        stack.Push(new TurtleState(state));
        Debug.Log("new top = " + stack.Peek());
    }

    public void pop()
    {
        Debug.Log("actual state = " + state);
        Debug.Log("stack.Peek() = " + stack.Peek());

        TurtleState temp = new TurtleState(stack.Pop());

        this.transform.position = temp.position;
        this.transform.rotation = Quaternion.Euler(temp.rotation);
        
        this.heading = temp.heading;
        this.pitch = temp.pitch;
        this.roll = temp.roll;
        this.radius = temp.radius;
        this.step = temp.step;

        this.position = temp.position;
        this.rotation = temp.rotation;

        Debug.Log("new state = " + state);
    }
    

    //Parsing methods
    public string getNextWord()
    {
        string word = "";
        // Debug.Log("inside getNextWord");
        // Debug.Log("wordPointer = " + wordPointer);
        // Debug.Log("production.Length = " + production.Length);
        // Debug.Log("production[wordPointer] = " + production[wordPointer]);
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
    }

    //run methods

    private void Start() {
        Debug.Log("production : "+production);
        
        state = new TurtleState();
        this.push();
        
        racine = new GameObject("racine");
        wordPointer = 0;

        this.transform.position = racine.transform.position = position;
        this.transform.rotation = racine.transform.rotation = Quaternion.Euler(rotation);
        
        Camera.main.transform.position = this.transform.position + new Vector3(0, 20, 5);
    }

    private void Update() {       
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //camera follow the turtle 
            //Camera.main.transform.position = this.transform.position + new Vector3(0, 0, -15);
           
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
            changeHead(-30);
        }
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            changeHead(30);
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            changeRoll(30);
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            changeRoll(-30);
        }
        if(Input.GetKeyDown(KeyCode.Z))
        {
            changePitch(30);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            changePitch(-30);
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