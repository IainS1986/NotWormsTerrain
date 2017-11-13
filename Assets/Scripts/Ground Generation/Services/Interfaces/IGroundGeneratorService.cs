using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGroundGeneratorService
{
    int[,] Generate(int width, int height);
    void DotRemoval(int xx, int yy, int ww, int hh, int tw, int th, ref int[,] ground);
    void RemoveDiagonals(int xx, int yy, int ww, int hh, ref int[,] ground);
    bool SafeGroundFillForGenerator(int x, int y, int r, int type, int tw, int th, ref int[,] ground);
}
