using UnityEngine;
using System.Collections.Generic;

public class LVisualizer : MonoBehaviour
{
    public LSystemGenerator lsystem;
    List<Vector3> positions = new List<Vector3>();

    public RoadHelper roadHelper;

    private int length = 8;
    private float angle = 90;

      public void Visualize(string sequence)
    {
        int currLength = length;
        positions.Clear();
        Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
        var currPos = Vector3.zero;
        Vector3 dir = Vector3.up;
        Vector3 tempPos = Vector3.zero;

        positions.Add(currPos);

        foreach (var letter in sequence) { 
            EncodingLetter encodingLetter = (EncodingLetter)letter;
            switch (encodingLetter)
            {
                case EncodingLetter.unknown:
                    Debug.LogWarning($"Unknown encoding letter: {letter}");
                    break;
                case EncodingLetter.save:
                    savePoints.Push(new AgentParameters { 
                        position = currPos,
                        direction = dir,
                        length = currLength
                    });
                    break;
                case EncodingLetter.load:
                    if (savePoints.Count > 0)
                    {
                        var parameters = savePoints.Pop();
                        currPos = parameters.position;
                        dir = parameters.direction;
                        currLength = parameters.length;
                    }
                    else
                    {
                         throw new System.InvalidOperationException("No saved state to load from.");
                    }
                        break;
                case EncodingLetter.draw:
                    tempPos = currPos;
                    currPos += dir * currLength;
                    roadHelper.PlaceRoad(tempPos, Vector3Int.RoundToInt(dir), currLength);
                    currLength = Mathf.Max(currLength - 2, 1);
                    positions.Add(currPos);
                    break;
                case EncodingLetter.turnRight:
                    dir = Quaternion.AngleAxis(angle, Vector3.forward) * dir;
                    break;
                case EncodingLetter.turnLeft:
                    dir = Quaternion.AngleAxis(-angle, Vector3.forward) * dir;
                    break;
            }
        }
        roadHelper.FixRoads();

    }

    

    public enum EncodingLetter { 
        unknown = '1',
        save = '[',
        load = ']',
        draw = 'F',
        turnLeft = '-',
        turnRight = '+',
    }
}
