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
    public int wordPointer = 0;

    public GameObject racine;

    public Turtle(Lsystem lsystem)
    {
        this.lsystem = lsystem;

        state = new TurtleState();

        turtle3D = new GameObject("turtle3D");
        racine = new GameObject("racine");

        turtle3D.transform.position = racine.transform.position = state.position;; 
        turtle3D.transform.rotation = racine.transform.rotation = Quaternion.Euler(state.heading, state.pitch, state.roll);
    }

    //getters of the turtle's state

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


    //turtle methods
    public void Move(float distance)
    {
        Vector3 newPosition = position + direction * distance;
        turtle3D.transform.position = newPosition;
        position = newPosition;
    }

    public void heading(float angle)
    {
        
    }

    public void pitch(float angle)
    {
        
    }

    public void roll(float angle)
    {
       
    }

    public void push()
    {
    }

    public void pop()
    {
        

    }
    

    //Parsing methods
    public string getNextWord()
    {
        string word = "";
        while (wordPointer < lsystem.current.Length && lsystem.current[wordPointer] != ' ')
        {
            word += lsystem.current[wordPointer];
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
            case == 'F' : 
                interpretF(param);
                break;

            case == 'h' :
                interpretHeading(param);
                break;
            case == 'p' :
                interpretPitch(param);
                break;
            case == 'r' :    
                interpretRoll(param);
                break;

            case == '[' : 
                interpretPush(param);
                break;
            case == ']' :
                interpretPop(param);
                break;

            default:
                interpretVariable(c);
        }
    }

    public void interpretF(string param)
    {
        if (param == "")
        {
            param = "1";
        }
        float distance = float.Parse(param);
        Move(distance);
    }
    
}