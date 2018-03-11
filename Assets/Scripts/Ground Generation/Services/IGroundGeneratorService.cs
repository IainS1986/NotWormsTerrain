using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services
{
    /// <summary>
    /// Ground Terrain Subservice used to generate the underlaying dots
    /// used to generate the terrain. This service will have some procedural
    /// elements to generate "nice" looking terrain at random
    /// </summary>
    public interface IGroundGeneratorService
    {
        /// <summary>
        /// Generates a random terrain of dots in the provided Ground object.
        /// </summary>
        /// <param name="ground">The ground object to generates the terrain in</param>
        void Generate(Ground ground);

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
        void DotRemoval(int xx, int yy, int ww, int hh, Ground ground);

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
        void RemoveDiagonals(int xx, int yy, int ww, int hh, Ground ground);

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
        bool SafeGroundFillForGenerator(int x, int y, int r, int type, Ground ground);
    }
}
