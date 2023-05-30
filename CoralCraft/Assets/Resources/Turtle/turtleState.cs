using UnityEngine;
using UnityEditor;

public class TurtleState
{
    public float heading { get; set; }
    public float pitch { get; set; }
    public float roll { get; set; }
    public float radius { get; set; }
    public float step { get; set; }

    public Vector3 position { get; set; } 
    public Vector3 rotation { get; set; }

    public TurtleState()
    {
        this.heading = 0;
        this.pitch = 0;
        this.roll = 0;
        this.radius = 0;
        this.step = 0;

        this.position = new Vector3(0, 0, 0);
        this.rotation = new Vector3(0, 0, 0);
    }

    public TurtleState(float heading, float pitch, float roll, float radius, float step, Vector3 position, Vector3 direction)
    {
        this.heading = heading;
        this.pitch = pitch;
        this.roll = roll;
        this.radius = radius;
        this.step = step;

        this.position = position;
        this.rotation = direction;
    }

    public TurtleState(TurtleState turtleState)
    {
        this.heading = turtleState.heading;
        this.pitch = turtleState.pitch;
        this.roll = turtleState.roll;
        this.radius = turtleState.radius;
        this.step = turtleState.step;

        this.position = turtleState.position;
        this.rotation = turtleState.rotation;
    }


    public override string ToString()
    {
        return " p : " + pitch + "h : " + heading + " r : " + roll +
         "\nR : " + radius + " step : " + step + 
         "\npos : " + position + 
         "\nrot : " + rotation;
    }

}