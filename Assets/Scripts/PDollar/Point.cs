
public class Point {

    public float X, Y;
    public int StrokeID;
    public PointMetadata metadata;

    public Point(float x, float y, int strokeId)
    {
        this.X = x;
        this.Y = y;
        this.StrokeID = strokeId;
    }

    public void SetMetaData(PointMetadata mdata)
    {
        metadata = mdata;
    }
}
