
using UnityEngine;

public static class RayDirections 
{
    private static Vector3 up = new Vector2(0f, 0.5f);
    private static Vector3 upLeft = new Vector2(-0.44f, 0.25f);
    private static Vector3 upRight = new Vector2(0.44f, 0.25f);
    private static Vector3 down = new Vector2(0f, -0.5f);
    private static Vector3 downLeft = new Vector2(-0.44f, -0.25f);
    private static Vector3 downRight = new Vector2(0.44f, -0.25f);



    public static Vector3 Up
    {
        get { return up; }
    }
    public static Vector3 UpLeft
    {
        get { return upLeft; }
    }
    public static Vector3 UpRight
    {
        get { return upRight; }
    }
    public static Vector3 Down
    {
        get { return down; }
    }
    public static Vector3 DownLeft
    {
        get { return downLeft; }
    }
    public static Vector3 DownRight
    {
        get { return downRight; }
    }

    public static Vector3 Directions(int i)
    {
        switch (i)
        {
            case 0:
                return up;
            case 1:
                return upLeft;
            case 2:
                return upRight;
            case 3:
                return down;
            case 4:
                return downLeft;
            case 5:
                return downRight;
        }

        return Vector3.zero;
    }
}
