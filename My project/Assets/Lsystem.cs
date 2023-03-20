
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Lsystem {

    public string axiom;
    public string current;
    public List<string> variables;
    public List<string> constantes;
    public Dictionary<char, List<Rule>> rules;

    public float angle;

    public Lsystem(List<string> variables, List<string> constantes, 
                    string axiom, Dictionary<char, List<Rule>> rules) {

        this.variables = variables;
        this.constantes = constantes;
        this.axiom = axiom;
        this.current = axiom;
        this.rules = rules;
        this.angle = 1f;
    }

    public void Generate(int n = 1)
    {
        for (int i = 0; i < n; i++)
        {
            string next = "";
            for (int j = 0; j < current.Length; j++)
            {
                char c = current[j];
                if (rules.ContainsKey(c))
                {
                    //choose a rule based on its probability
                    float r = UnityEngine.Random.value;
                    float sum = 0f;
                    foreach (Rule rule in rules[c])
                    {
                        sum += rule.getProbability();
                        if (r <= sum)
                        {
                            next += rule.getRule();
                            break;
                        }
                    }
                }
                else
                {
                    next += c;
                }
            }
            current = next;
        }

        Debug.Log("generated string = " + current);
    }


    public override string ToString()
    {
        string str = "";

        str += "variables = " + string.Join(", ", variables) + "\n";
        str += "constantes = " + string.Join(", ", constantes) + "\n";
        str += "axiom = " + axiom + "\n";
        str += "angle = " + angle + "\n";
        str += "rules = \n";
        foreach (KeyValuePair<char, List<Rule>> rule in rules)
        {
            foreach (Rule r in rule.Value)
            {
                str += rule.Key + " -> " + r.getRule() + " (" + r.getProbability() + ")\n";
            }
        }
        str += "current = " + current + "\n";

        return str;
    }
}
