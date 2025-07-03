using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleVisualizer : MonoBehaviour {
    public LSystemGenerator lsystem;
    private List<Vector3> positions = new List<Vector3>();

    public RoadHelper roadHelper;

    private int length = 1;
    private float angle = 90;

    public int Length {
        get {
            if (length > 0) {
                return length;
            }
            else {
                return 1;
            }
        }
        set {
            length = value;
        }
    }

    public void VisualiseSequence(string sequence) {
        positions.Clear();
        
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
        Vector3 currPos = Vector3.zero;
        
        Vector3Int direction = Vector3Int.up;
        Vector3 tempPos = Vector3.zero;
        
        positions.Add(currPos);

        foreach (var letter in sequence) {
            EncodingLetters encoding = (EncodingLetters)letter;
            switch (encoding) {
                case EncodingLetters.Unknown:
                    break;
                case EncodingLetters.Save:
                    savePoints.Push(new AgentParameters {
                        position = currPos,
                        direction = direction,
                        length = Length
                    });
                    break;
                case EncodingLetters.Load:
                    if (savePoints.Count > 0) {
                        AgentParameters agentParameter = savePoints.Pop();
                        currPos = agentParameter.position;
                        direction = agentParameter.direction;
                        length = agentParameter.length;
                    }
                    else {
                        throw new SystemException("No such savepoint");
                    }
                    break;
                case EncodingLetters.Draw:
                    tempPos = currPos;
                    currPos += direction * length;
                    roadHelper.PlaceRoads(tempPos, Vector3Int.RoundToInt(direction),  length);
                    positions.Add(currPos);
                    break;
                case EncodingLetters.TRight:
                    direction = new Vector3Int(-direction.y, direction.x, 0);;
                    break;
                case EncodingLetters.TLeft:
                    direction = new Vector3Int(direction.y, -direction.x, 0);

                    break;
                
            }
        }
    }
    
    public enum EncodingLetters {
        Unknown = '1',
        Save = '[',
        Load = ']',
        Draw = 'F',
        TRight = '+',
        TLeft = '-',
    }

}
