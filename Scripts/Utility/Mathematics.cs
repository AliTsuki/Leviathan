using System;

// Helpful math functions
public static class Mathematics
{
    public static float RoundToNearestHalf(float _input)
    {
        return (float)(Math.Round(_input * 2, MidpointRounding.AwayFromZero) / 2);
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

    public Float2(float _x, float _y)
    {
        this.x = _x;
        this.y = _y;
    }

    public void Set(float _x, float _y)
    {
        this.x = _x;
        this.y = _y;
    }

    public void SetX(float _x)
    {
        this.x = _x;
    }

    public void SetY(float _y)
    {
        this.y = _y;
    }

    public override string ToString()
    {
        return $@"({this.x}, {this.y})";
    }
}
