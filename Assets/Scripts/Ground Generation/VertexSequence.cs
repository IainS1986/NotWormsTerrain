using System;
using System.Collections.Generic;

public class VertexSequence
{
    private List<Point> Vertices { get; set; }

    public VertexSequence(){
        Vertices = new List<Point>();
    }
    
    public Point this[int i]{
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
    
    public void Add(Point a){
        Vertices.Add(a);
    }
    
    public bool Remove(Point a){
        return Vertices.Remove(a);
    }

    public void CopyFrom(VertexSequence vs)
    {
        Vertices = vs.Vertices;
    }
}