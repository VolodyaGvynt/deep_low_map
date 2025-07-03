using UnityEngine;
using System.Collections.Generic;

public class LVisualizer : MonoBehaviour
{
    public LSystemGenerator lsystem;
    List<Vector3> positions = new List<Vector3>();
    public GameObject prefab;
    public Material lineMaterial;

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
                    DrawLine(tempPos, currPos, Color.red);
                    Debug.Log($"Drawing line of lenght {currLength}");
                    currLength = Mathf.Max(currLength - 2, 1);
                    positions.Add(currPos);
                    break;
                case EncodingLetter.turnLeft:
                    dir = Quaternion.AngleAxis(angle, Vector3.forward) * dir;
                    break;
                case EncodingLetter.turnRight:
                    dir = Quaternion.AngleAxis(-angle, Vector3.forward) * dir;
                    break;
            }
        }

        foreach (var pos in positions)
        {
           Instantiate(prefab, pos, Quaternion.identity);         
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject lineObject = new GameObject("Line");
        lineObject.transform.position = start;
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
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
