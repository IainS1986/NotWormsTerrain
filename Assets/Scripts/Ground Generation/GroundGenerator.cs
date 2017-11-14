using System.Collections;
using System;
using System.Collections.Generic;
using Poly2Tri.Triangulation.Polygon;
using Poly2Tri.Triangulation.Delaunay.Sweep;
using Poly2Tri.Triangulation;
using System.Linq;
using UnityEngine;
using Common;
using Poly2Tri.Triangulation.Delaunay;

public class GroundGenerator 
{
    public enum GroundStage
    {
        NONE = 0,
        DOTS,
        MARCHING,
        SMOOTHED,
        VERTEX_REMOVAL,
        DECOMP,
        MESH,
        LIPS,
    };

    IGroundGeneratorService m_groundGeneratorService;
    IMarchingService m_marchingSquaresService;
    IContourSmoothingService m_contourSmoothingService;
    IDecompService m_decompService;

    public int[,] Ground { get; private set; }
    public int[,] GroundToChunk { get; private set; }
    public Dictionary<int, GroundChunk> IDToChunk { get; private set; }
    public int Height { get; private set; }
    public int Width { get; private set; }
    public List<GroundChunk> Chunks { get; private set; }
    public GroundStage CurrentStage { get; set; }
    
	public GroundGenerator(int _width, int _height)
    {
		Height = _height;
		Width = _width;
        Ground = new int[Height, Width];
        GroundToChunk = new int[Height, Width];
        IDToChunk = new Dictionary<int, GroundChunk>();
        CurrentStage = GroundStage.NONE;
        Chunks = new List<GroundChunk>();

        //TODO TinyIOC
        m_groundGeneratorService = new GroundGeneratorService();
        m_contourSmoothingService = new ContourSmoothingService();
        m_marchingSquaresService = new MarchingService();
        m_decompService = new DecompService();
    }

    //Helper Function for external API
    public bool GroundWillChange(int x, int y, int s, int type)
    {
        for (int i = x; i <= x + s; i++)
        {
            for (int j = y; j <= y + s; j++)
            {
                int xx = i;
                int yy = j;
                if (xx < 0 || xx >= Width) continue;
                if (yy < 0 || yy >= Height) continue;
                if (Ground[yy,xx] == type) continue;
                return true;
            }
        }
        return false;
    }

    public void GroundChangeSelectiveRebuild(int x, int y, int s, int type)
    {
        bool change = m_groundGeneratorService.SafeGroundFillForGenerator(x, y, s, type,Width,Height, ref Ground);

        if (!change)
            return;

        if (CurrentStage == GroundStage.NONE) return;
        if (CurrentStage == GroundStage.DOTS) return;

        int border = 2;//Extra range to check for nearby ground that *may* be effected
        int minx = int.MaxValue;
        int miny = int.MaxValue;

        //Check and remove effected Chunks
        Dictionary<int, bool> chunkIdsToRemove = new Dictionary<int, bool>();
        for (int i = 0; i < s + (border * 2); i++)
        {
            int xx = x - border + i;
            for (int j = 0; j < s + (border * 2); j++)
            {
                int yy = y - border + j;

                //Ensure we don't go out of bounds
                if (xx >= 0 && xx < Width && yy >= 0 && yy < Height)
                {
                    if (xx < minx) minx = xx;
                    if (yy < miny) miny = yy;

                    if (GroundToChunk[yy, xx] != 0 && !chunkIdsToRemove.ContainsKey(GroundToChunk[yy, xx]))
                    {
                        chunkIdsToRemove.Add(GroundToChunk[yy, xx], true);
                    }
                }
            }
        }

        //Clear ChunkID lookups
        foreach(var id in chunkIdsToRemove)
        {
            GroundChunk chunk = IDToChunk[id.Key];
            //Destroy chunk
            Chunks.Remove(chunk);
            IDToChunk.Remove(id.Key);
        }
        //Clear GroundToChunk values (Quicker way to do this?)
        for (int a = 0; a < Width; a++)
            for (int b = 0; b < Height; b++)
                if (chunkIdsToRemove.ContainsKey(GroundToChunk[b, a]))
                    GroundToChunk[b, a] = 0;

        //Preprocess
        m_groundGeneratorService.DotRemoval(minx, miny, s + (border * 2), s + (border * 2), Width, Height, ref Ground);
        m_groundGeneratorService.RemoveDiagonals(minx, miny, s + (border * 2), s + (border * 2), ref Ground);

        List<GroundChunk> chunks = m_marchingSquaresService.March(0,0,Width, Height, Ground, ref GroundToChunk, ref IDToChunk);

        if (CurrentStage >= GroundStage.SMOOTHED)
        {
            m_contourSmoothingService.SmoothContours(ref Chunks);  
        }
        if (CurrentStage >= GroundStage.VERTEX_REMOVAL)
        {
            m_contourSmoothingService.RemoveVertices(ref Chunks);
        }
        if (CurrentStage >= GroundStage.DECOMP)
        {
            m_decompService.Decomp(ref Chunks);
        }

        Chunks.AddRange(chunks);
    }

    public void Generate()
    {
        Ground = m_groundGeneratorService.Generate(Width, Height);
        Chunks = new List<GroundChunk>();
        GroundToChunk = new int[Height, Width];
        IDToChunk = new Dictionary<int, GroundChunk>();

        CurrentStage = GroundStage.DOTS;
    }

    public void March()
    {
        GroundToChunk = new int[Height, Width];
        IDToChunk = new Dictionary<int, GroundChunk>();
        Chunks = m_marchingSquaresService.March(0, 0, Width, Height, Ground, ref GroundToChunk, ref IDToChunk);

        CurrentStage = GroundStage.MARCHING;
    }

    public void SmoothContours()
    {
        if (Chunks == null)
            return;

        m_contourSmoothingService.SmoothContours(ref Chunks);

        CurrentStage = GroundStage.SMOOTHED;
    }

    public void RemoveVertices()
    {
        if (Chunks == null)
            return;

        m_contourSmoothingService.RemoveVertices(ref Chunks);

        CurrentStage = GroundStage.VERTEX_REMOVAL;
    }

    public void Decomp()
    {
        if (Chunks == null)
            return;

        m_decompService.Decomp(ref Chunks);

        CurrentStage = GroundStage.DECOMP;
    }
}
