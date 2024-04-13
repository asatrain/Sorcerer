using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignEdge : MonoBehaviour
{
    [SerializeField] private float yScale;
    
    public void Place(SignVertex vertex1, SignVertex vertex2)
    {
        var pos1 = vertex1.transform.position;
        var pos2 = vertex2.transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.right, pos2 - pos1);
        transform.position = (pos1 + pos2) / 2;
        var scale = transform.localScale;
        scale.x = (pos2 - pos1).magnitude;
        scale.y = yScale;
        transform.localScale = scale;
    }
}
