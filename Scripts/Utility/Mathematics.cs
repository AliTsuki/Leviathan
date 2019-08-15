using System;

// Helpful math functions
public static class Mathematics
{
    // Rounds to nearest half
    public static float RoundToNearestHalf(float input)
    {
        return (float)(Math.Round(input * 2, MidpointRounding.AwayFromZero) / 2);
    }
}


// Contains 2 floats
public class Float2
{
    public float x { get; set; }
    public float y { get; set; }

    public Float2()
    {
        this.x = 0f;
        this.y = 0f;
    }

    public Float2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public void Set(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetX(float x)
    {
        this.x = x;
    }

    public void SetY(float y)
    {
        this.y = y;
    }

    public override string ToString()
    {
        return $@"({this.x}, {this.y})";
    }
}
