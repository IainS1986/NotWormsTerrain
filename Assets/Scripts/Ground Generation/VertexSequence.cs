using System;
using System.Collections.Generic;
using UnityEngine;

public class VertexSequence
{
    private List<Vector2> Vertices { get; set; }

    public VertexSequence(){
        Vertices = new List<Vector2>();
    }
    
    public Vector2 this[int i]{
        get
        {
            if (i < 0)
                i += Vertices.Count;

            return Vertices[i % Vertices.Count];
        }
        set
        {
            if (i < 0)
                i += Vertices.Count;

            Vertices[i % Vertices.Count] = value;
        }
    }
    
    public int Count{
        get{ return Vertices.Count;}
    }
    
    public void Add(Vector2 a){
        Vertices.Add(a);
    }
    
    public bool Remove(Vector2 a){
        return Vertices.Remove(a);
    }

    public void CopyFrom(VertexSequence vs)
    {
        Vertices = vs.Vertices;
    }
}