using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SignEdge : MonoBehaviour
{
    [SerializeField] private float yScale;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;
    
    private readonly SignVertex[] vertices = new SignVertex[2];

    public bool Active => vertices.All(vertex => vertex.Active);

    private void Start()
    {
        GameManager.Instance.RegisterEdge(this);
    }

    private void Update()
    {
        spriteRenderer.color = Active ? activeColor : inactiveColor;
    }

    public void Connect(SignVertex vertex1, SignVertex vertex2)
    {
        var pos1 = vertex1.transform.position;
        var pos2 = vertex2.transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.right, pos2 - pos1);
        var edgePos = (pos1 + pos2) / 2;
        edgePos.z += .1f;
        transform.position = edgePos;
        var scale = transform.localScale;
        scale.x = (pos2 - pos1).magnitude;
        scale.y = yScale;
        transform.localScale = scale;

        vertices[0] = vertex1;
        vertices[1] = vertex2;
    }

    public void Intersect()
    {
        if (!GameManager.Instance.GameActive) return;
        if (!Active) return;

        foreach (var signVertex in vertices)
        {
            signVertex.Deactivate();
        }
    }
}