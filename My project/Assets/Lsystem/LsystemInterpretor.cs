using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class LsystemInterpretor
{
    public static Lsystem ParseFile(string filePath){
        string[] lines = File.ReadAllLines(filePath);

        // Parse the variables, constants, start symbol, and angle
        List<string> variables = parseList(lines[0]);
        List<string> constants = parseList(lines[1]);
        string startSymbol = parseStartSymbol(lines[2]);
        float angle = parseAngle(lines[3]);

        // Parse the production rules
        Dictionary<char, List<Rule>> rules = new Dictionary<char, List<Rule>>();
        for (int i = 4; i < lines.Length; i++)
        {
            if (lines[i].Length == 0 || lines[i].Contains("rules") || lines[i][0] == '#')
            {
                continue;
            }
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

        Lsystem lsystem = new Lsystem(variables, constants, startSymbol, rules, angle);
        return lsystem;
    }

    
    // methods for parsing the grammar file

    private static List<string> parseList(string line)
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

    private static string parseStartSymbol(string line)
    {
        string startSymbol = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
        //Debug.Log("parsed start symbol = " + startSymbol);
        return startSymbol;
    }

    private static float parseAngle(string line)
    {
        float angle = float.Parse(line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim());
        //Debug.Log("parsed angle = " + angle);
        return angle;
    }

}
