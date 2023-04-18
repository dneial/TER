using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class LsystemInterpretorV2
{
    public static LsystemV2 ParseFile(string filePath){
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
            //if line is "rules :", emptyor starts with '#' skip it
            if (lines[i].Length == 0 || lines[i].Contains("rules") || lines[i][0] == '#')
            {
                continue;
            }

            string[] rule = lines[i].Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
            //rule = { "A", "r(180,180)", "F0.5", "H" }}

            char variable = rule[0][0]; // "A"
            string production = "";     // -> "r(180,180) F0.5 H"

            for (int j = 1; j < rule.Length; j++)
            {
                production += rule[j] + " ";
            }
            //delete last space
            production = production.Substring(0, production.Length - 1);
        
            rules[variable] = new List<Rule>();
            rules[variable].Add(new Rule(production));
        }

        LsystemV2 lsystem = new LsystemV2(variables, constants, startSymbol, rules, angle);
        return lsystem;
    }

    
    // methods for parsing the grammar file


    //parse the variables and constants
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
        return list;
    }

    //parse the starting axiom
    private static string parseStartSymbol(string line)
    {
        string startSymbol = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
        return startSymbol;
    }

    private static float parseAngle(string line)
    {
        float angle = float.Parse(line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim().Replace('.', ',') );
        return angle;
    }

}
