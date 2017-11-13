using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGeneratorService : IGroundGeneratorService
{
    private int[] m_chunk = new int[] { 2, 2 };
    private int[] m_passes = new int[] { 256, 128 };
    private int[] m_moves = new int[] { 20, 10 };
    private int[] m_sizes = new int[] { 10, 5 };

    private int m_totalMaterials = 2;//TODO Expose

    public int[,] Generate(int width, int height)
    {
        var ground = new int[height, width];
        var seed = (int)System.DateTime.Now.Ticks;

        System.Random r = new System.Random(seed);

        for (int i = 0; i < m_totalMaterials; i++)
        {
            //Number of chunks
            int chunk = m_chunk[i];

            //Number of passes per chunk
            int pass = m_passes[i];
            int move = m_moves[i];
            int size = m_sizes[i];

            for (int c = 0; c < m_chunk.Length; c++)
            {
                //Starting point
                int x = (c + 1) * (width / (chunk + 1));
                int y = r.Next(height / 3) + i * (height / 2);
                for (int p = 0; p < pass; p++)
                {
                    x += r.Next(-move, move);
                    y += r.Next(-move / 2, move / (i + 1));

                    SafeGroundFillForGenerator(x, y, size, i + 1, width, height, ref ground);
                }
            }
        }

        DotRemoval(0, 0, width, height, width, height, ref ground);
        RemoveDiagonals(0, 0, width, height, ref ground);

        return ground;
    }

    public void DotRemoval(int xx, int yy, int ww, int hh, int tw, int th, ref int[,] ground)
    {
        for (int x = xx; x < xx + ww; x++)
        {
            for (int y = yy; y < yy + hh; y++)
            {
                if (x > 0 && ground[y, x] == ground[y, x - 1]) continue;//Left
                if (x < tw - 1 && ground[y, x] == ground[y, x + 1]) continue;//Right
                if (y > 0 && ground[y, x] == ground[y - 1, x]) continue;//Up
                if (y < th - 1 && ground[y, x] == ground[y + 1, x]) continue;//Down
                if (x > 0 && y > 0 && ground[y, x] == ground[y - 1, x - 1]) continue;//TopLeft
                if (x < tw - 1 && y > 0 && ground[y, x] == ground[y - 1, x + 1]) continue;//TopRight
                if (x > 0 && y < th - 1 && ground[y, x] == ground[y + 1, x - 1]) continue;//BotLeft
                if (x < tw - 1 && y < th - 1 && ground[y, x] == ground[y + 1, x + 1]) continue;//BotRight
                //Remove
                ground[y, x] = ground[y, x - 1];
            }
        }
    }

    public void RemoveDiagonals(int xx, int yy, int ww, int hh, ref int[,] ground)
    {
        for (int x = xx; x < xx + ww; x++)
        {
            for (int y = yy; y < yy + hh; y++)
            {
                if (ground[y, x] == ground[y + 1, x + 1] &&
                    ground[y, x] != ground[y, x + 1] &&
                    ground[y, x] != ground[y + 1, x])
                {
                    //Remove diagonals
                    ground[y, x] = ground[y, x + 1];
                    ground[y + 1, x + 1] = ground[y + 1, x];
                }
            }
        }
    }

    public bool SafeGroundFillForGenerator(int x, int y, int r, int type, int tw, int th, ref int[,] ground)
    {
        bool change = false;
        int rpow = r * r;

        for (int i = -r; i < r; i++)
        {
            for (int j = -r; j < r; j++)
            {
                int xx = x + i;
                int yy = y + j;

                if (xx < 1 || xx >= tw - 1)
                    continue;

                if (yy < 1 || yy >= th - 1)
                    continue;

                if ((xx - x) * (xx - x) + (yy - y) * (yy - y) > rpow)
                    continue;

                if (ground[yy, xx] == type)
                    continue;

                ground[yy, xx] = type;
                change = true;
            }
        }
        return change;
    }
}
