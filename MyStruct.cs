
public struct MyFloatColor
{
    public float R;
    public float G;
    public float B;
}

public struct MyByteColor
{
    public byte R;
    public byte G;
    public byte B;
}

public struct edge
{
    public int a;
    public int b;
    public float w;
}

public struct ImprovedEdge
{
    public int a;
    public float w;
}
public class MyShortPoint
{
    public System.Int16 x;
    public System.Int16 y;
    public MyShortPoint(int x, int y)
    {
        this.x = (short)x;
        this.y = (short)y;
    }
}
public class Elipse
{
    public double[] a = new double[6];
    //a[5]*x^2+a[4]*xy+a[3]*y^2+a[2]*x+a[1]*y+a[0]-ellipse!

}
public class ElipseToDraw
{
    public int x;
    public int y;
    public int w;
    public int h;
    public ElipseToDraw(double x1, double y1, double w1, double h1)
    {
        x=(int)x1;
        y = (int)y1;
        w = (int)w1;
        h = (int)h1;
        
    }

}