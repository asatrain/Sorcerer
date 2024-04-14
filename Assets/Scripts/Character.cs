using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    private HashSet<Collider2D> enteredEdges = new();
    private HashSet<Collider2D> enteredVertices = new();

    private void Update()
    {
        if (transform.position.y < 0)
        {
            GameManager.Instance.Restart();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SignVertex"))
        {
            enteredVertices.Add(other);
            other.GetComponent<SignVertex>().Trigger();
        }
        else if (other.CompareTag("SignEdge"))
        {
            enteredEdges.Add(other);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("SignEdge") && enteredVertices.Count == 0)
        {
            other.GetComponent<SignEdge>().Intersect();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SignVertex"))
        {
            enteredVertices.Remove(other);
        }
        else if (other.CompareTag("SignEdge"))
        {
            enteredEdges.Remove(other);
        }
    }
}