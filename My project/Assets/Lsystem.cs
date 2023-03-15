
using System;
using System.Collections;
using System.Collections.Generic;

class Lsystem {

    public string axiom;
    public string current;
    public List<string> variables;
    public List<string> constantes;
    public Dictionary<char, string> rules = new Dictionary<char, string>();

    public Lsystem(List<string> variables, List<string> constantes, 
                    string axiom, Dictionary<char, string> rules) {

        this.variables = variables;
        this.constantes = constantes;
        this.axiom = axiom;
        this.current = axiom;
        this.rules = rules;
    }

    public void Generate(int n = 1) {
        for(int i = 0; i < n; i++) {
        string next = "";
        for (int j = 0; j < current.Length; j++) {
            char c = current[j];
            if (rules.ContainsKey(c)) {
                next += rules[c];
            } else {
                next += c;
            }
        }
        current = next;
        }
    }
}
