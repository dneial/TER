
using System;
using System.Collections;
using System.Collections.Generic;

class Lsystem {

    public string axiom;
    public string current;
    public Dictionary<char, string> rules = new Dictionary<char, string>();

    public Lsystem(string axiom, Dictionary<char, string> rules) {
        this.axiom = axiom;
        this.current = axiom;
        this.rules = rules;
    }

    public void Generate(int n = 1) {
        string next = "";
        //for (int i = 0; i < n; i++){
            for (int j = 0; j < current.Length; j++) {
                char c = current[j];
                if (rules.ContainsKey(c)) {
                    next += rules[c];
                } else {
                    next += c;
                }
            }
            current = next;
        //}
    }
}

class MainClass {
    public static void Main(string[] args) {
        Dictionary<char, string> rules = new Dictionary<char, string>();
        rules.Add('F', "FF+[+F-F-F]-[-F+F+F]");
        Lsystem lsystem = new Lsystem("F", rules);
        lsystem.Generate(3);
        Console.WriteLine(lsystem.current);
    }
}