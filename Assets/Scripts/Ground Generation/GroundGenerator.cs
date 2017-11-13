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

public class Decomp
{
    public List<Point> Points;
    public int[] Tris;
}

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


    private int TotalMaterials { get; set; }
	private int Seed { get; set; }

    private int[] m_chunk = new int[]{2,2};
    private int[] m_passes = new int[]{256, 128};
    private int[] m_moves = new int[]{20,10};
    private int[] m_sizes = new int[]{10,5};
 
    public int[,] Ground { get; private set; }
    public int[,] GroundToChunk { get; private set; }
    public Dictionary<int, GroundChunk> IDToChunk { get; private set; }
    public int Height { get; private set; }
    public int Width { get; private set; }

    public int[] m_smoothWeights = new int[] { 1, 2, 1 };

    public List<GroundChunk> Chunks { get; private set; }
    public GroundStage CurrentStage { get; set; }

    public int this[int y, int x]{
        get
        {
            if(y<0 || y >= Height || x < 0 || x >= Width || Ground == null)
                return -1;

            return Ground[y,x];
        }
        set
        { 
            if(y<0 || y >= Height || x < 0 || x >= Width || Ground == null)
                return;

            Ground[y,x] = value;
        }
    }

	public GroundGenerator(){}
	public GroundGenerator(int _width, int _height)
    {
        TotalMaterials = 2;
		Height = _height;
		Width = _width;
        Ground = new int[Height, Width];
        GroundToChunk = new int[Height, Width];
        IDToChunk = new Dictionary<int, GroundChunk>();
        CurrentStage = GroundStage.NONE;
        Chunks = new List<GroundChunk>();
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
                if (this[yy,xx] == type) continue;
                return true;
            }
        }
        return false;
    }

    public void GroundChangeSelectiveRebuild(int x, int y, int s, int type)
    {
        bool change = SafeGroundFill(x, y, s, type);

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
        DotRemoval(minx, miny, s + (border * 2), s + (border * 2));
        RemoveDiagonals(minx, miny, s + (border * 2), s + (border * 2));

        List<GroundChunk> chunks = March(0, 0, Width, Height);

        if (CurrentStage >= GroundStage.SMOOTHED)
        {
            SmoothContours(chunks);     
        }
        if (CurrentStage >= GroundStage.VERTEX_REMOVAL)
        {
            RemoveVertices(chunks);
        }
        if (CurrentStage >= GroundStage.DECOMP)
        {
            Decomp(chunks);
        }

        Chunks.AddRange(chunks);
    }

    public void Generate()
    {
        Seed = (int)System.DateTime.Now.Ticks;
        Ground = new int[Height, Width];
        Chunks = new List<GroundChunk>();
        GroundToChunk = new int[Height, Width];
        IDToChunk = new Dictionary<int, GroundChunk>();

        System.Random r = new System.Random(Seed);

        for(int i=0; i < TotalMaterials; i++)
        {
            //Number of chunks
            int chunk = m_chunk[i];
            
            //Number of passes per chunk
            int pass = m_passes[i];
            int move = m_moves[i];
            int size = m_sizes[i];
            
            for(int c = 0; c < m_chunk.Length; c++)
            {
                //Starting point
                int x = (c+1)*(Width/(chunk+1));
                int y = r.Next(Height/3) + i*(Height/2);
                for(int p=0; p < pass; p++)
                {
                    x += r.Next(-move,move);
                    y += r.Next(-move/2,move/(i+1));

                    SafeGroundFillForGenerator(x, y, size, i+1);
                }
            }
        }

        DotRemoval(0,0,Width,Height);
        RemoveDiagonals(0, 0, Width, Height);

        CurrentStage = GroundStage.DOTS;
    }

    public void DotRemoval(int xx, int yy, int ww, int hh)
    {
        for (int x = xx; x < xx + ww; x++)
        {
            for (int y = yy; y < yy + hh; y++)
            {
                if (x > 0 && this[y,x] == this[y,x - 1]) continue;//Left
                if (x < Width - 1 && this[y,x] == this[y,x + 1]) continue;//Right
                if (y > 0 && this[y,x] == this[y - 1,x]) continue;//Up
                if (y < Height - 1 && this[y,x] == this[y + 1,x]) continue;//Down
                if (x > 0 && y > 0 && this[y,x] == this[y - 1,x - 1]) continue;//TopLeft
                if (x < Width - 1 && y > 0 && this[y,x] == this[y - 1,x + 1]) continue;//TopRight
                if (x > 0 && y < Height - 1 && this[y,x] == this[y + 1,x - 1]) continue;//BotLeft
                if (x < Width - 1 && y < Height - 1 && this[y,x] == this[y + 1,x + 1]) continue;//BotRight
                //Remove
                this[y, x] = this[y, x - 1];
            }
        }
    }

    public void RemoveDiagonals(int xx, int yy, int ww, int hh)
    {
        for (int x = xx; x < xx + ww; x++)
        {
            for (int y = yy; y < yy + hh; y++)
            {
                if(this[y,x] == this[y + 1, x + 1] &&
                    this[y,x] != this[y, x + 1] &&
                    this[y,x] != this[y + 1, x])
                {
                    //Remove diagonals
                    this[y,x] = this[y, x + 1];
                    this[y + 1,x + 1] = this[y + 1, x];
                }
            }
        }
    }

    public void March()
    {
        GroundToChunk = new int[Height, Width];
        IDToChunk = new Dictionary<int, GroundChunk>();
        Chunks = March(0, 0, Width, Height);
        CurrentStage = GroundStage.MARCHING;
    }

    public List<GroundChunk> March(int xx, int yy, int ww, int hh)
    {
        List<GroundChunk> chunks = new List<GroundChunk>();

        for (int y=yy; y < yy + hh; y++)
        {
            for (int x=xx; x < xx + ww; x++)
            {
                int g = this[y,x];
                int c = GroundToChunk[y,x];

                if(g == 0 || c != 0)
                    continue;

                //Get Left, Below and Left/Below Diag values to see if they are the same
                //This is used to check holes and know which chunk to assign holes too
                int lg = this[y,x-1];
                int bg = this[y-1,x];
                int lbg = this[y-1,x-1];

                bool neighbourSame = lg == g || bg == g || lbg == g;

                //Is this an edge
                int marchVal = MarchingValue(x, y, g);
                if(marchVal!=0 && marchVal!=15)
                {
                    //New edge....get the contour and either make a new Ground Chunk or assign it as a Hole to an existing one
                    GroundChunk owner = null;

                    int cID = 0;
                    if(neighbourSame)
                    {
                        if(lg == g)
                            cID = GroundToChunk[y,x-1];
                        else if(bg == g)
                            cID = GroundToChunk[y-1,x];
                        else if(lbg==g)
                            cID = GroundToChunk[y-1,x-1];
                        else
                            UnityEngine.Debug.LogWarning("NO NEIGHBOUR FOUND");

                        owner = IDToChunk[cID];
                    }
                    else
                    {
                        cID = GroundChunk.NextID;
                        owner = new GroundChunk();
                        owner.GroundType = g;
                        IDToChunk.Add(cID, owner);

                        chunks.Add(owner);
                    }

                    //Get Contour
                    VertexSequence contour = FindContour(x, y, g, cID, owner);

                    if(neighbourSame)
                        owner.Holes.Add(contour);
                    else
                        owner.Edge = contour;

                }
                else if(neighbourSame)
                {
                    //Mark this non edge as being "owned" by the neighbouring chunk (i.e. its inside a chunk)
                    if(lg == g)
                        GroundToChunk[y,x] = GroundToChunk[y,x-1];
                    else if(bg == g)
                        GroundToChunk[y,x] = GroundToChunk[y-1,x];
                    else if(lbg==g)
                        GroundToChunk[y,x] = GroundToChunk[y-1,x-1];
                }
            }
        }

        return chunks;
    }

    private VertexSequence FindContour (int x, int y, int g, int chunkID, GroundChunk owner)
    {   
        //Contour
        VertexSequence contour = new VertexSequence();
        //Found a dot to trace
        int startx = x; int starty = y;
        int curx = x; int cury = y;
        int prevx = -1; int prevy = -1;
        int nextx = -1; int nexty = -1;
        int bits = 0;
        bool addPoint = true;

        //Predict an error with Marching Squares here
        bits = MarchingValue(curx, cury, g);
        if(bits == 6 || bits == 9) UnityEngine.Debug.LogWarning("MARCHING MIGHT CRASH");
        if(bits == 15) UnityEngine.Debug.LogWarning("MARCHING IS STARTING IN ERROR!!!");
        
        do
        {
            nextx = curx;
            nexty = cury;

            bits = MarchingValue(curx, cury, g);
            addPoint = true;
            switch(bits){
                case  1: nexty -= 1 ; break;
                case  2: nextx += 1; break;
                case  3: nextx += 1; break;
                case  4: nextx -= 1; break;
                case  5: nexty -= 1; break;
                case  6: if(prevx == curx && prevy == cury + 1){ nextx -= 1; } else { nextx += 1; } break;//What if prev is null as we have just started, we could actually go the wrong way
                case  7: nextx += 1; break;
                case  8: nexty += 1; break;
                case  9: if(prevx == curx - 1 && prevy == cury){ nexty -= 1; } else { nexty += 1; } break;//What if prev is null as we have just started, we could actually go the wrong way
                case 10: nexty += 1; break;
                case 11: nexty += 1; break;
                case 12: nextx -= 1; break;
                case 13: nexty -= 1; break;
                case 14: nextx -= 1; break;
                case 15: nextx -= 1; addPoint=false; break;
                    
                default: throw new ArgumentException(string.Format("Value was {0} prev was {1}, {2}", bits, prevx, prevy));
            }
            
            if(addPoint)
            {
                if(contour.Count==0)
                {
                    startx = curx;
                    starty = cury;
                }

                if(this[cury, curx] == g)
                    GroundToChunk[cury, curx] = chunkID;

                contour.Add(new Point(){X = curx-0.5f, Y = cury-0.5f});
                prevx = curx;
                prevy = cury;
            }
            
            curx = nextx;
            cury = nexty;
        }
        while(curx!=startx || cury!=starty);
        
        return contour;
    }

    private int MarchingValue(int x, int y, int i) {
        int sum = 0;

        if (this [y - 1, x - 1] == i)
            sum |= 1;

        if (this [y - 1, x] == i)
            sum |= 2;

        if (this [y, x - 1] == i)
            sum |= 4;

        if (this [y, x] == i)
            sum |= 8;

        return sum;
    }

    private bool SafeGroundFill(int x, int y, int r, int type)
    {
        bool change = false;
        for (int i = 0; i <= r; i++)
        {
            for (int j = 0; j <= r; j++)
            {
                int xx = x + i;
                int yy = y + j;

                if (xx < 0 || xx >= Width)
                    continue;

                if (yy < 0 || yy >= Height)
                    continue;

                if (this[yy, xx] == type)
                    continue;

                this[yy, xx] = type;
                change = true;
            }
        }
        return change;
    }

    //Fills in a "circle" radius, other function is pure square fill...
    private bool SafeGroundFillForGenerator(int x, int y, int r, int type)
    {
        bool change = false;
        int rpow = r * r;

        for(int i=-r; i<r; i++)
        {
            for(int j=-r; j<r; j++)
            {
                int xx = x+i;
                int yy = y+j;

                if(xx < 1 || xx >= Width - 1)
                    continue;

                if(yy < 1 || yy >= Height - 1) 
                    continue;

                if((xx-x)*(xx-x) + (yy-y)*(yy-y) > rpow)
                    continue;

                if (this[yy, xx] == type)
                    continue;

                this[yy, xx] = type;
                change = true;
            }
        }
        return change;
    }

    public void SmoothContours()
    {
        if (Chunks == null)
            return;

        SmoothContours(Chunks);

        CurrentStage = GroundStage.SMOOTHED;
    }

    private void SmoothContours(List<GroundChunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            chunk.Poly = null;
            //Smooth Contours
            SmoothContour(chunk.Edge);
            SmoothContour(chunk.Edge);
            SmoothContour(chunk.Edge);
            SmoothContour(chunk.Edge);
            SmoothContour(chunk.Edge);
            foreach (var hole in chunk.Holes)
            {
                SmoothContour(hole);
                SmoothContour(hole);
                SmoothContour(hole);
                SmoothContour(hole);
                SmoothContour(hole);
            }
        }
    }

    private void SmoothContour(VertexSequence vs)
    {
        VertexSequence smooth = new VertexSequence();
        for (int i = 0; i < vs.Count; i++)
        {
            float x = 0;
            float y = 0;
            int w = 0;
            for (int j = -m_smoothWeights.Length / 2; j <= m_smoothWeights.Length / 2; j++)
            {
                x += vs[i + j].X * m_smoothWeights[j + (m_smoothWeights.Length / 2)];
                y += vs[i + j].Y * m_smoothWeights[j + (m_smoothWeights.Length / 2)];
                w += m_smoothWeights[j + (m_smoothWeights.Length / 2)];
            }
            x /= w;
            y /= w;
            smooth.Add(new Point() { X = x, Y = y });
        }

        vs.CopyFrom(smooth);
    }

    public void RemoveVertices()
    {
        if (Chunks == null)
            return;

        RemoveVertices(Chunks);

        CurrentStage = GroundStage.VERTEX_REMOVAL;
    }

    private void RemoveVertices(List<GroundChunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            chunk.Poly = null;
            //Smooth Contours
            RemoveVertices(chunk.Edge);
            foreach (var hole in chunk.Holes)
                RemoveVertices(hole);
        }
    }

    private void RemoveVertices(VertexSequence vs)
    {
        int count = 0;
        for (int i = 0; i < vs.Count; i++)
        {
            Point a = vs[i - 1];
            Point b = vs[i];
            Point c = vs[i + 1];

            //Remove b if its the same X or same Y as a and c
            bool abX = UnityEngine.Mathf.Approximately(a.X, b.X);
            bool bcX = UnityEngine.Mathf.Approximately(b.X, c.X);
            bool abY = UnityEngine.Mathf.Approximately(a.Y, b.Y);
            bool bcY = UnityEngine.Mathf.Approximately(b.Y, c.Y);
            if ((abX && bcX) || (abY && bcY))
            {
                count++;
                vs.Remove(b);
                i--;
            }
        }
    }

    public void Decomp()
    {
        if (Chunks == null)
            return;

        Decomp(Chunks);

        CurrentStage = GroundStage.DECOMP;
    }

    private void Decomp(List<GroundChunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            Decomp(chunk);
        }
    }

    //TODO Drop vertex point ect and just use PolygonPoint throughout?
    private void Decomp(GroundChunk chunk)
    {
        List<PolygonPoint> edge = new List<PolygonPoint>();
        for (int i = 0; i < chunk.Edge.Count; i++)
            edge.Add(new PolygonPoint(chunk.Edge[i].X, chunk.Edge[i].Y));

        List<Polygon> holes = new List<Polygon>();
        if (chunk.Holes != null)
        {
            for(int i=0; i  <chunk.Holes.Count; i++)
            {
                List<PolygonPoint> hole = new List<PolygonPoint>();
                for (int j = 0; j < chunk.Holes[i].Count; j++)
                    hole.Add(new PolygonPoint(chunk.Holes[i][j].X, chunk.Holes[i][j].Y));

                holes.Add(new Polygon(hole));
            }
        }

        Polygon poly = new Polygon(edge);
        for (int i = 0; i < holes.Count; i++)
            poly.AddHole(holes[i]);


        //Triangulate!
        P2T.Triangulate(poly);
        IList<TriangulationPoint> points = poly.Points;
        IList<DelaunayTriangle> tris = poly.Triangles;

        List<Point> finalPoints = new List<Point>();
        int[] finalTris = new int[tris.Count * 3];
        Dictionary<TriangulationPoint, int> pointToIndex = new Dictionary<TriangulationPoint, int>();
        for (int t = 0; t < tris.Count; t++)
        {
            DelaunayTriangle tri = tris[t];
            for (int i = 0; i < 3; i++)
            {
                //check if the points are in the dictionary, if not add it with the next index
                int val = 0;
                if (pointToIndex.TryGetValue(tri.Points[i], out val))
                {
                    finalTris[t * 3 + i] = val;
                }
                else
                {
                    finalPoints.Add(new Point() { X = tri.Points[i].Xf, Y = tri.Points[i].Yf });
                    pointToIndex[tri.Points[i]] = finalPoints.Count - 1;
                    finalTris[t * 3 + i] = finalPoints.Count - 1;
                }
            }
        }


        Decomp result = new Decomp();
        result.Points = finalPoints;
        result.Tris = finalTris;
        chunk.Poly = result;
    }

}
