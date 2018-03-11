using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services.Concrete
{
    /// <summary>
    /// Ground Terrain Subservice used to generate the underlaying dots
    /// used to generate the terrain. This service will have some procedural
    /// elements to generate "nice" looking terrain at random
    /// 
    /// This generator will generate a random starting point for a material type
    /// and then "paint" circles of different sizes in an iterative pattern from this
    /// point. This will also try and spread each chunk of material evenly across the terrain.
    /// 
    /// The terrain generated will be random every time.
    /// </summary>
    public class GroundGeneratorService : IGroundGeneratorService
    {
        /// <summary>
        /// How many chunks of terrain do we want to randomly produces for each material type.
        /// </summary>
        private int[] m_chunk = new int[] { 2, 2 };

        /// <summary>
        /// How many passes are run for each material, this is the number of circles/blotches
        /// to add for each piece of terrain of that material type
        /// </summary>
        private int[] m_passes = new int[] { 256, 128 };

        /// <summary>
        /// The max range (positive and negative) to "move" between passes in the ground
        /// when generating. (both X and Y movement)
        /// </summary>
        private int[] m_moves = new int[] { 20, 10 };

        /// <summary>
        /// The sizes of the circles/blotches used when generating the terrain
        /// </summary>
        private int[] m_sizes = new int[] { 10, 5 };

        /// <summary>
        /// Total number of materials to add to the terrain
        /// </summary>
        private int m_totalMaterials = 2;//TODO Expose

        /// <summary>
        /// Generates a random terrain of dots in the provided Ground object.
        /// </summary>
        /// <param name="ground">The ground object to generates the terrain in</param>
        public void Generate(Ground ground)
        {
            if(ground.Chunks!= null)
            {
                foreach (var chunks in ground.Chunks)
                    chunks.Value.Dispose();
            }

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

        /// <summary>
        /// An optimisation function used to tidy up bits of the terrain that could pose
        /// problems during generation. Namely this will remove single dots of one terrain type
        /// that could not produce ground chunks of a standard worth dealing with.
        /// </summary>
        /// <param name="xx">The x index in the terrain to start scanning</param>
        /// <param name="yy">The y index in the terrain to start scanning</param>
        /// <param name="ww">The number of dots in the width to scan for</param>
        /// <param name="hh">The number of dots in the height to scan for</param>
        /// <param name="ground">The Ground object to scan and remove dots in the terrain for</param>
        public void DotRemoval(int xx, int yy, int ww, int hh, Ground ground)
        {
            for (int x = xx; x < xx + ww; x++)
            {
                if (x >= ground.Width)
                    continue;

                for (int y = yy; y < yy + hh; y++)
                {
                    if (y >= ground.Height)
                        continue;

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

        /// <summary>
        /// An optimisation function used to tidy up bits of the terrain that could pose
        /// problems during generation. Namely this will remove diagonal dots that don't
        /// scan nicely into meshes. For example,
        /// o X
        /// X o
        /// 
        /// This will remove this Diagonal result by turning it into something like...
        /// X X
        /// X X
        /// </summary>
        /// <param name="xx">The x index in the terrain to start scanning</param>
        /// <param name="yy">The y index in the terrain to start scanning</param>
        /// <param name="ww">The number of dots in the width to scan for</param>
        /// <param name="hh">The number of dots in the height to scan for</param>
        /// <param name="ground">The Ground object to scan and remove dots in the terrain for</param>
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

        /// <summary>
        /// A helper function to "fill" a section of the ground terrain into a specified ground
        /// type. It will not throw an array out of bounds exception as it will be safely clamped
        /// to the terrain boundary.
        /// </summary>
        /// <param name="x">The X index to start filling from</param>
        /// <param name="y">The Y index to start filling from</param>
        /// <param name="r">The radius in a circle from (x, y) to fill</param>
        /// <param name="type">The ground type to fill with</param>
        /// <param name="ground">The ground object to fill terrain in</param>
        /// <returns>TRUE if any dots in the terrain were altered, otherwise FALSE</returns>
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
}
