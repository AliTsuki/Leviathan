using System;

// Helpful math functions
public static class Mathematics
{
    public static float RoundToNearestHalf(float _input)
    {
        return (float)(Math.Round(_input * 2, MidpointRounding.AwayFromZero) / 2);
    }
}
