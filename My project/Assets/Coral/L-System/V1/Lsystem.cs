
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lsystem {

    public string axiom;
    public string current;
    public List<string> variables;
    public List<string> constantes;
    public Dictionary<char, List<Rule>> rules;

    public float angle;

    public Lsystem(List<string> variables, List<string> constantes, 
                    string axiom, Dictionary<char, List<Rule>> rules, float angle = 1f) {

        this.variables = variables;
        this.constantes = constantes;
        this.axiom = axiom;
        this.current = axiom;
        this.rules = rules;
        this.angle = angle;
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
            // Debug.Log("current = " + current);
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

    //traduire la chaine de caractère de lsystem v1 en lsystem v2
    public void trad(float thickness, float length, float angle)
    {

        string t = thickness.ToString();
        string l = length.ToString();
        string a = angle.ToString();
        
        string tmp = "F0 ";
        int cpt = 0;

        for(int i = 0; i < current.Length; i++)
        {
            //si on trouve une variable
            if(variables.Contains(current[i].ToString()))
            {
                tmp += "F" + l + " ";
            }
            else if(current[i] == '+')
            {
               tmp += "h" + a + " ";
            }
            else if(current[i] == '-')
            {
               tmp += "h-" + a + " ";
            }
            else if(current[i] == '&')
            {
               tmp += "r" + a + " ";
            }
            else if(current[i] == '^')
            {
               tmp += "r-" + a + " ";
            }
            else if(current[i] == '<')
            {
               tmp += "p" + a + " ";
            }
            else if(current[i] == '>')
            {
               tmp += "p-" + a + " ";
            }
            else if (current[i] == '|')
            {
                tmp += "h180 ";
            }
            else if(current[i] == '[')
            {
                if(i != 0 )
                {
                    tmp += "[ ";
                    cpt++;
                }
            }
            else if(current[i] == ']')
            {
                if(cpt > 0) 
                {
                    tmp += "] ";
                    cpt--;
                }
            }
            else 
            {
                Debug.Log("erreur de traduction ? : " + current[i]);
            }
        }
        //retirer le dernier espace
        tmp = tmp.Substring(0, tmp.Length - 1);
        this.current = tmp;

        Debug.Log("current = " + this.current);
    }

}
