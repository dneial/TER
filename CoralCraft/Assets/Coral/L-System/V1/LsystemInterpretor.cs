using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LsystemInterpretor
{
    public static Lsystem ParseFile(string filePath){
        string format = filePath.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[1];

        string[] lines = File.ReadAllLines(filePath);

        // Parse the variables, constants, start symbol, and angle
        List<string> variables = parseList(lines[0]);
        List<string> constants = parseList(lines[1]);
        string startSymbol = parseStartSymbol(lines[2]);
        float angle = parseAngle(lines[3]);

        // Parse the production rules
        Dictionary<char, List<Rule>> rules = new Dictionary<char, List<Rule>>();

        string production = "";
        float probability = 1f;

        if(format == "lsys") 
        {
            for (int i = 4; i < lines.Length; i++)
            {
                //if line is "rules :", empty or starts with '#' skip it
                if (lines[i].Length == 0 || lines[i].Contains("rules") || lines[i][0] == '#')
                {
                    continue;
                }

                string[] rule = lines[i].Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
                //rule = { "A", "B[-A]C[+A]B" }
                //ou
                //rule = { "A", "(0,5)", "B[-A]C[+A]B" }

                if (rule.Length != 0)
                {
                    char variable = rule[0][0]; //"A"
                    rules[variable] = new List<Rule>();
                    if(rule.Length == 2) //  { "A", "B[-A]C[+A]B" }
                    {
                        production = rule[1];
                        rules[variable].Add(new Rule(production));
                    }
                    else{ //{ "A", "(0,5)", "B[-A]C[+A]B" }
                        production = rule[2]; //"B[-A]C[+A]B"
                        probability = float.Parse(rule[1].Trim(new char[] { '(', ')' })); //0,5
                        rules[variable].Add(new Rule(production, probability));
                    }
                }
            }
        } else if(format == "lsys2") {
            for (int i = 4; i < lines.Length; i++)
            {
                //if line is "rules :", empty or starts with '#' skip it
                if (lines[i].Length == 0 || lines[i].Contains("rules") || lines[i][0] == '#')
                {
                    continue;
                }

                string[] rule = lines[i].Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
                //rule = { "A", "r(180,180)", "F0.5", "H" }
                //rule = { "A", "(0,5)", "r(180,180)", "F0.5", "H" }

                char variable = rule[0][0]; // "A"
                production = "";
                
                if(rule[1][0] != '(') //  { "A", "r(180,180)", "F0.5", "H" }
                {
                    for (int j = 1; j < rule.Length; j++)
                    {
                        production += rule[j] + " ";
                        //production = "r(180,180) F0.5 H "
                    }
                    
                }
                else //{ "A", "(0,5)", "r(180,180)", "F0.5", "H" }
                {                

                    for (int j = 2; j < rule.Length; j++)
                    {
                        production += rule[j] + " ";
                        //production = "r(180,180) F0.5 H "
                    }
                    //debug : print production
                    //Debug.Log("prod = " + production + "'");
                    probability = float.Parse(rule[1].Trim(new char[] { '(', ')' }).Replace('.', ',')); //0,5
                }

                //delete last space in production
                production = production.Substring(0, production.Length - 1);
                rules[variable] = new List<Rule>();
                rules[variable].Add(new Rule(production, probability));
            }
        }

        return new Lsystem(variables, constants, startSymbol, rules, angle);;
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
        float angle = float.Parse(line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim().Replace('.', ','));
        //Debug.Log("parsed angle = " + angle);
        return angle;
    }

}
