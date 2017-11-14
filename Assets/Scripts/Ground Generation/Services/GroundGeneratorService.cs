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

    public void Generate(Ground ground)
    {
        ground.Chunks = new Dictionary<int, GroundChunk>();
        ground.Dots = new Dot[ground.Height, ground.Width];
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
                int x = (c + 1) * (ground.Width / (chunk + 1));
                int y = r.Next(ground.Height / 3) + i * (ground.Height / 2);
                for (int p = 0; p < pass; p++)
                {
                    x += r.Next(-move, move);
                    y += r.Next(-move / 2, move / (i + 1));

                    SafeGroundFillForGenerator(x, y, size, i + 1, ground);
                }
            }
        }

        DotRemoval(0, 0, ground.Width, ground.Height, ground);
        RemoveDiagonals(0, 0, ground.Width, ground.Height, ground);
    }

    public void DotRemoval(int xx, int yy, int ww, int hh, Ground ground)
    {
        for (int x = xx; x < xx + ww; x++)
        {
            for (int y = yy; y < yy + hh; y++)
            {
                if (x > 0 && ground.Dots[y, x].Value == ground.Dots[y, x - 1].Value) continue;//Left
                if (x < ground.Width - 1 && ground.Dots[y, x].Value == ground.Dots[y, x + 1].Value) continue;//Right
                if (y > 0 && ground.Dots[y, x].Value == ground.Dots[y - 1, x].Value) continue;//Up
                if (y < ground.Height - 1 && ground.Dots[y, x].Value == ground.Dots[y + 1, x].Value) continue;//Down
                if (x > 0 && y > 0 && ground.Dots[y, x].Value == ground.Dots[y - 1, x - 1].Value) continue;//TopLeft
                if (x < ground.Width - 1 && y > 0 && ground.Dots[y, x].Value == ground.Dots[y - 1, x + 1].Value) continue;//TopRight
                if (x > 0 && y < ground.Height - 1 && ground.Dots[y, x].Value == ground.Dots[y + 1, x - 1].Value) continue;//BotLeft
                if (x < ground.Width - 1 && y < ground.Height - 1 && ground.Dots[y, x].Value == ground.Dots[y + 1, x + 1].Value) continue;//BotRight
                //Remove
                ground.Dots[y, x] = ground.Dots[y, x - 1];
            }
        }
    }

    public void RemoveDiagonals(int xx, int yy, int ww, int hh, Ground ground)
    {
        for (int x = xx; x < xx + ww; x++)
        {
            for (int y = yy; y < yy + hh; y++)
            {
                if (x >= ground.Width - 1 ||
                    y >= ground.Height - 1)
                    continue;

                if (ground.Dots[y, x].Value == ground.Dots[y + 1, x + 1].Value &&
                    ground.Dots[y, x].Value != ground.Dots[y, x + 1].Value &&
                    ground.Dots[y, x].Value != ground.Dots[y + 1, x].Value)
                {
                    //Remove diagonals
                    ground.Dots[y, x].Value= ground.Dots[y, x + 1].Value;
                    ground.Dots[y + 1, x + 1].Value = ground.Dots[y + 1, x].Value;
                }
            }
        }
    }

    public bool SafeGroundFillForGenerator(int x, int y, int r, int type, Ground ground)
    {
        bool change = false;
        int rpow = r * r;

        for (int i = -r; i < r; i++)
        {
            for (int j = -r; j < r; j++)
            {
                int xx = x + i;
                int yy = y + j;

                if (xx < 1 || xx >= ground.Width - 1)
                    continue;

                if (yy < 1 || yy >= ground.Height - 1)
                    continue;

                if ((xx - x) * (xx - x) + (yy - y) * (yy - y) > rpow)
                    continue;

                if (ground.Dots[yy, xx].Value == type)
                    continue;

                ground.Dots[yy, xx].Value = type;
                change = true;
            }
        }
        return change;
    }
}
