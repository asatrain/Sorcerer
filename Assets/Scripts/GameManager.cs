using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void GameOverHandler();

public class GameManager : Singleton<GameManager>
{
    private readonly List<SignEdge> signEdges = new();
    private Controls controls;
    public bool GameActive { private set; get; } = true;
    public event GameOverHandler GameOver;

    protected override void Awake()
    {
        base.Awake();
        controls = new Controls();
        controls.Player.Restart.performed += _ => Restart();
    }

    private void Update()
    {
        if (signEdges.Count > 0 && signEdges.All(edge => edge.Active))
        {
            GameActive = false;
            GameOver?.Invoke();
        }
    }

    public void RegisterEdge(SignEdge signEdge)
    {
        signEdges.Add(signEdge);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}