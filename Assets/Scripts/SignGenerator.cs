using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SignEdgeVertices
{
    private SignVertex vertex1;
    private SignVertex vertex2;
}

public class SignGenerator : MonoBehaviour
{
    [SerializeField] private List<SignEdgeVertices> edges;
}
