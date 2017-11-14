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
    IGroundGeneratorService m_groundGeneratorService;
    IMarchingService m_marchingSquaresService;
    IContourSmoothingService m_contourSmoothingService;
    IDecompService m_decompService;

    public Ground Ground
    {
        get;
        private set;
    }
    
	public GroundGenerator(int _width, int _height)
    {
        m_groundGeneratorService = new GroundGeneratorService();
        m_contourSmoothingService = new ContourSmoothingService();
        m_marchingSquaresService = new MarchingService();
        m_decompService = new DecompService();

        Ground = new Ground(_width, _height);
    }

    public bool GroundChangeSelectiveRebuild(int x, int y, int s, int type)
    {
        bool change = m_groundGeneratorService.SafeGroundFillForGenerator(x, y, s, type, Ground);

        if (!change)
            return change;

        if (Ground.CurrentStage == GroundStage.NONE) return change;
        if (Ground.CurrentStage == GroundStage.DOTS) return change;

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
                if (xx >= 0 && xx < Ground.Width && yy >= 0 && yy < Ground.Height)
                {
                    if (xx < minx) minx = xx;
                    if (yy < miny) miny = yy;

                    if (Ground.DotToChunk[yy, xx] != 0 && !chunkIdsToRemove.ContainsKey(Ground.DotToChunk[yy, xx]))
                    {
                        chunkIdsToRemove.Add(Ground.DotToChunk[yy, xx], true);
                    }
                }
            }
        }

        //Clear ChunkID lookups
        foreach(var id in chunkIdsToRemove)
        {
            GroundChunk chunk = Ground.IDToChunk[id.Key];
            //Destroy chunk
            Ground.Chunks.Remove(chunk);
            Ground.IDToChunk.Remove(id.Key);
        }
        //Clear GroundToChunk values (Quicker way to do this?)
        for (int a = 0; a < Ground.Width; a++)
            for (int b = 0; b < Ground.Height; b++)
                if (chunkIdsToRemove.ContainsKey(Ground.DotToChunk[b, a]))
                    Ground.DotToChunk[b, a] = 0;

        //Preprocess
        m_groundGeneratorService.DotRemoval(minx, miny, s + (border * 2), s + (border * 2), Ground);
        m_groundGeneratorService.RemoveDiagonals(minx, miny, s + (border * 2), s + (border * 2), Ground);

        List<GroundChunk> chunks = m_marchingSquaresService.March(0, 0, Ground.Width, Ground.Height, Ground);

        if (Ground.CurrentStage >= GroundStage.SMOOTHED)
        {
            m_contourSmoothingService.SmoothContours(chunks);  
        }
        if (Ground.CurrentStage >= GroundStage.VERTEX_REMOVAL)
        {
            m_contourSmoothingService.RemoveVertices(chunks);
        }
        if (Ground.CurrentStage >= GroundStage.DECOMP)
        {
            m_decompService.Decomp(chunks);
        }

        Ground.Chunks.AddRange(chunks);

        return change;
    }

    public void Generate()
    {
        m_groundGeneratorService.Generate(Ground);
    
        Ground.CurrentStage = GroundStage.DOTS;
    }

    public void March()
    {
        Ground.DotToChunk = new int[Ground.Height, Ground.Width];
        Ground.IDToChunk = new Dictionary<int, GroundChunk>();
        Ground.Chunks = m_marchingSquaresService.March(0, 0, Ground.Width, Ground.Height, Ground);

        Ground.CurrentStage = GroundStage.MARCHING;
    }

    public void SmoothContours()
    {
        if (Ground == null)
            return;

        m_contourSmoothingService.SmoothContours(Ground);

       Ground.CurrentStage = GroundStage.SMOOTHED;
    }

    public void RemoveVertices()
    {
        if (Ground == null)
            return;

        m_contourSmoothingService.RemoveVertices(Ground);

        Ground.CurrentStage = GroundStage.VERTEX_REMOVAL;
    }

    public void Decomp()
    {
        if (Ground == null)
            return;

        m_decompService.Decomp(Ground);

        Ground.CurrentStage = GroundStage.DECOMP;
    }
}
