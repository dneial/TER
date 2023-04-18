using System.Collections.Generic;
using System.IO;

public class Config {
    public string name;
    public float thickness;
    public float length;
    public int nbIteration;
    public float angle;
    public float noise;
    public float branch_chance;
    public string grammar;

    public Config(float thickness, float length, string name, int nbIteration, float angle, float noise, float branch_chance, string grammar){
        this.name = name;   
        this.thickness = thickness;
        this.length = length;
        this.nbIteration = nbIteration;
        this.angle = angle;
        this.noise = noise;
        this.branch_chance = branch_chance;
        this.grammar = grammar;
    }
}