namespace DesktopApp.Models
{
    public struct Vector2F
    {
        public float X;
        public float Y;

        public Vector2F(float x, float y)
        {
            X = x;
            Y = y;
        }
        
        public static Vector2F operator- (Vector2F v1, Vector2F v2)
        {
            return new Vector2F(v1.X - v2.X, v1.Y - v2.Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}