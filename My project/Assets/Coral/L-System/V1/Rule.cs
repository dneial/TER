using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule {
    private float probability;
    private string rule;

    public Rule(string rule, float probability = 1f) {
          
        this.probability = probability;
        this.rule = rule;
    }

    public float getProbability() {
        return this.probability;
    }

    public string getRule() {
        return this.rule;
    }
}