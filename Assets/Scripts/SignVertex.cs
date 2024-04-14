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
    [SerializeField] private float gameOverHideDuration;
    private float activeTimeLeft;

    public bool Active => activeTimeLeft > 0;

    private void Start()
    {
        InitEdges();
        GameManager.Instance.GameOver += OnGameOver;
    }

    private void Update()
    {
        if (!GameManager.Instance.GameActive) return;
        
        UpdateTimer();
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
            signEdge.Connect(this, connectedVertex);
            CreatedEdges.Add(edgeVertices);
        }
    }

    private void UpdateTimer()
    {
        var material = timerSpriteRenderer.material;
        material.SetFloat(ArcProperty, ((activeDuration - activeTimeLeft) / activeDuration) * 360);
        timerSpriteRenderer.material = material;
    }

    public void Trigger()
    {
        if (!GameManager.Instance.GameActive) return;

        if (Active)
        {
            activeTimeLeft = 0;
        }
        else
        {
            activeTimeLeft = activeDuration;
            UpdateTimer();
        }
    }

    public void Deactivate()
    {
        activeTimeLeft =  0;
    }

    private void OnGameOver()
    {
        StartCoroutine(HideTimer());
    }

    private IEnumerator HideTimer()
    {
        while (timerSpriteRenderer.color.a > 0)
        {
            var color = timerSpriteRenderer.color;
            float smoothVelocity = 0;
            color.a = Mathf.SmoothDamp(color.a, -0.01f, ref smoothVelocity, gameOverHideDuration);
            timerSpriteRenderer.color = color;
            yield return new WaitForEndOfFrame();
        }
    }
}