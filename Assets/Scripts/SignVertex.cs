using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SignVertex : MonoBehaviour
{
    private static readonly HashSet<HashSet<SignVertex>> CreatedEdges = new(HashSet<SignVertex>.CreateSetComparer());
    private static readonly int ArcProperty = Shader.PropertyToID("_Arc1");
    
    [SerializeField] private List<SignVertex> connectedVertices;
    [SerializeField] private GameObject edge;
    [SerializeField] private SpriteRenderer timerSpriteRenderer;
    [SerializeField] private float activeDuration;
    private float activeTimeLeft;

    public bool Active => activeTimeLeft > 0;

    private void Start()
    {
        InitEdges();
    }

    private void Update()
    {
        var material = timerSpriteRenderer.material;
        material.SetFloat(ArcProperty, ((activeDuration - activeTimeLeft) / activeDuration) * 360);
        timerSpriteRenderer.material = material;

        activeTimeLeft -= Time.deltaTime;
    }

    private void InitEdges()
    {
        foreach (var connectedVertex in connectedVertices)
        {
            var edgeVertices = new HashSet<SignVertex> { this, connectedVertex };
            if (CreatedEdges.Contains(edgeVertices))
            {
                Debug.LogWarning($"Duplicate edge of {this} and {connectedVertex} ignored");
                continue;
            }
        
            var edgeObject = Instantiate(edge);
            var signEdge = edgeObject.GetComponent<SignEdge>();
            signEdge.Place(this, connectedVertex);
            CreatedEdges.Add(edgeVertices);
        }
    }

    public void Trigger()
    {
        activeTimeLeft = Active ? 0 : activeDuration;
    }
}