using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingService : IMarchingService
{
    public List<GroundChunk> March(int xx, int yy, int ww, int hh, int[,] ground, ref int[,] groundToChunk, ref Dictionary<int, GroundChunk> idToChunk)
    {
        List<GroundChunk> chunks = new List<GroundChunk>();

        for (int y = yy; y < yy + hh; y++)
        {
            for (int x = xx; x < xx + ww; x++)
            {
                int g = ground[y, x];
                int c = groundToChunk[y, x];

                if (g == 0 || c != 0)
                    continue;

                //Get Left, Below and Left/Below Diag values to see if they are the same
                //This is used to check holes and know which chunk to assign holes too
                int lg = ground[y, x - 1];
                int bg = ground[y - 1, x];
                int lbg = ground[y - 1, x - 1];

                bool neighbourSame = lg == g || bg == g || lbg == g;

                //Is this an edge
                int marchVal = MarchingValue(x, y, g, ground);
                if (marchVal != 0 && marchVal != 15)
                {
                    //New edge....get the contour and either make a new Ground Chunk or assign it as a Hole to an existing one
                    GroundChunk owner = null;

                    int cID = 0;
                    if (neighbourSame)
                    {
                        if (lg == g)
                            cID = groundToChunk[y, x - 1];
                        else if (bg == g)
                            cID = groundToChunk[y - 1, x];
                        else if (lbg == g)
                            cID = groundToChunk[y - 1, x - 1];
                        else
                            UnityEngine.Debug.LogWarning("NO NEIGHBOUR FOUND");

                        owner = idToChunk[cID];
                    }
                    else
                    {
                        cID = GroundChunk.NextID;
                        owner = new GroundChunk();
                        owner.GroundType = g;
                        idToChunk.Add(cID, owner);

                        chunks.Add(owner);
                    }

                    //Get Contour
                    VertexSequence contour = FindContour(x, y, g, cID, owner, ground, ref groundToChunk);

                    if (neighbourSame)
                        owner.Holes.Add(contour);
                    else
                        owner.Edge = contour;

                }
                else if (neighbourSame)
                {
                    //Mark this non edge as being "owned" by the neighbouring chunk (i.e. its inside a chunk)
                    if (lg == g)
                        groundToChunk[y, x] = groundToChunk[y, x - 1];
                    else if (bg == g)
                        groundToChunk[y, x] = groundToChunk[y - 1, x];
                    else if (lbg == g)
                        groundToChunk[y, x] = groundToChunk[y - 1, x - 1];
                }
            }
        }

        return chunks;
    }

    private VertexSequence FindContour(int x, int y, int g, int chunkID, GroundChunk owner, int[,] ground, ref int[,] groundToChunk)
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
        bits = MarchingValue(curx, cury, g, ground);
        if (bits == 6 || bits == 9) UnityEngine.Debug.LogWarning("MARCHING MIGHT CRASH");
        if (bits == 15) UnityEngine.Debug.LogWarning("MARCHING IS STARTING IN ERROR!!!");

        do
        {
            nextx = curx;
            nexty = cury;

            bits = MarchingValue(curx, cury, g, ground);
            addPoint = true;
            switch (bits)
            {
                case 1: nexty -= 1; break;
                case 2: nextx += 1; break;
                case 3: nextx += 1; break;
                case 4: nextx -= 1; break;
                case 5: nexty -= 1; break;
                case 6: if (prevx == curx && prevy == cury + 1) { nextx -= 1; } else { nextx += 1; } break;//What if prev is null as we have just started, we could actually go the wrong way
                case 7: nextx += 1; break;
                case 8: nexty += 1; break;
                case 9: if (prevx == curx - 1 && prevy == cury) { nexty -= 1; } else { nexty += 1; } break;//What if prev is null as we have just started, we could actually go the wrong way
                case 10: nexty += 1; break;
                case 11: nexty += 1; break;
                case 12: nextx -= 1; break;
                case 13: nexty -= 1; break;
                case 14: nextx -= 1; break;
                case 15: nextx -= 1; addPoint = false; break;

                default: throw new ArgumentException(string.Format("Value was {0} prev was {1}, {2}", bits, prevx, prevy));
            }

            if (addPoint)
            {
                if (contour.Count == 0)
                {
                    startx = curx;
                    starty = cury;
                }

                if (ground[cury, curx] == g)
                    groundToChunk[cury, curx] = chunkID;

                contour.Add(new Point() { X = curx - 0.5f, Y = cury - 0.5f });
                prevx = curx;
                prevy = cury;
            }

            curx = nextx;
            cury = nexty;
        }
        while (curx != startx || cury != starty);

        return contour;
    }

    private int MarchingValue(int x, int y, int i, int[,] ground)
    {
        int sum = 0;

        if (ground[y - 1, x - 1] == i)
            sum |= 1;

        if (ground[y - 1, x] == i)
            sum |= 2;

        if (ground[y, x - 1] == i)
            sum |= 4;

        if (ground[y, x] == i)
            sum |= 8;

        return sum;
    }
}
