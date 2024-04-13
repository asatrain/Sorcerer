using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SignVertex : MonoBehaviour
{
    [SerializeField] private List<SignVertex> connectedVertices;
    [SerializeField] private GameObject edge;
    
    private static readonly HashSet<HashSet<SignVertex>> CreatedEdges = new(HashSet<SignVertex>.CreateSetComparer());

    private void Start()
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
}