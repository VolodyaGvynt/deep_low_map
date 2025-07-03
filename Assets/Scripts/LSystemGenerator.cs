using System;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class LSystemGenerator : MonoBehaviour {
    public Rule[] rules;
    public string rootSentence;
    [Range(0,10)]
    public int iterationLimit = 1;

    public bool randIgnoreRule = true;
    [Range(0, 1)] 
    public float ignoreChance = 0.3f;
    

    private void Start() {
        Debug.Log(GenerateSequence());
    }

    public string GenerateSequence(string word = null) {
        if (word == null) {
            word = rootSentence;
        }

        return RecGrow(word);
    }

    private string RecGrow(string word, int currIteration = 0) {
        if (currIteration >= iterationLimit) {
            return word;
        }

        StringBuilder newWord = new StringBuilder();

        foreach (var c in word) {
            newWord.Append(c);
            RecPrcessRule(newWord, c, currIteration);
        }

        return newWord.ToString();
    }

    private void RecPrcessRule(StringBuilder newWord, char c, int currIteration) {
        foreach (var rule in rules) {
            if (rule.letter == c.ToString()) {
                if (randIgnoreRule) {
                    if (Random.value < ignoreChance && currIteration > 1) {
                        return;
                    }
                }

                newWord.Append(RecGrow(rule.GetResult(), currIteration + 1));
            }
        }
    }
}
