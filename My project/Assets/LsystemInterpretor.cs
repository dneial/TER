using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class LsystemInterpretor
{
    private Lsystem lsystem;
    private int nbIteration;
    private string grammarFilePath;


    public LsystemInterpretor(string grammarFilePath, Lsystem lsystem, int nbIteration)
    {
        this.lsystem = lsystem;
        this.nbIteration = nbIteration;
        this.grammarFilePath = grammarFilePath;

    }

    //method that takes a file containing a grammar and modifies the lsystem object accordingly
    public void interpret()
    {
        // Read the grammar file
        string[] lines = File.ReadAllLines(this.grammarFilePath);

        // Parse the variables, constants, start symbol, and angle
        List<string> variables = parseList(lines[0]);
        List<string> constants = parseList(lines[1]);
        string startSymbol = parseStartSymbol(lines[2]);
        float angle = parseAngle(lines[3]);

        // Parse the production rules
        Dictionary<char, List<Rule>> rules = new Dictionary<char, List<Rule>>();
        for (int i = 4; i < lines.Length; i++)
        {
            string[] rule = lines[i].Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
            if (rule.Length != 0)
            {
                char variable = rule[0][0];
                rules[variable] = new List<Rule>();
                if(rule.Length == 2)
                {
                    string production = rule[1];
                    rules[variable].Add(new Rule(production));
                }
                else{
                    string production = rule[2];
                    float probability = float.Parse(rule[1].Trim(new char[] { '(', ')' }));
                    rules[variable].Add(new Rule(production, probability));
                }
            }
        }

        // Update the Lsystem object
        this.lsystem.variables = variables;
        this.lsystem.constantes = constants;
        this.lsystem.axiom = startSymbol;
        this.lsystem.current = startSymbol;
        this.lsystem.rules = rules;
        this.lsystem.angle = angle;

        // Generate the L-system
        // this.lsystem.Generate(this.nbIteration);
    }

    // methods for parsing the grammar file

    private List<string> parseList(string line)
    {
        //Split line by ':' and ',' and remove empty entries
        string[] tokens = line.Split(new char[] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
        
        List<string> list = new List<string>();

        //Add all tokens to the list (except the first one "variables", "constants", etc.)
        for (int i = 1; i < tokens.Length; i++)
        {
            list.Add(tokens[i].Trim());
        }

        //Debug.Log("parsed list = " + String.Join(", ", list.ToArray()));
        return list;
    }

    private string parseStartSymbol(string line)
    {
        string startSymbol = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
        //Debug.Log("parsed start symbol = " + startSymbol);
        return startSymbol;
    }

    private float parseAngle(string line)
    {
        float angle = float.Parse(line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim());
        //Debug.Log("parsed angle = " + angle);
        return angle;
    }

    

}
