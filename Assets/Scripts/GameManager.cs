using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void GameOverHandler();

public class GameManager : Singleton<GameManager>
{
    private readonly List<SignEdge> signEdges = new();
    public bool GameActive { private set; get; } = true;
    public event GameOverHandler GameOver;

    private void Update()
    {
        if (signEdges.All(edge => edge.Active))
        {
            GameActive = false;
            GameOver?.Invoke();
        }
    }

    public void RegisterEdge(SignEdge signEdge)
    {
        signEdges.Add(signEdge);
    }
}