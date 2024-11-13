using SkiaSharp;

public static class Xibao
{
    public static void Main()
    {
        File.WriteAllBytes("output.png", Generate(true, "喜报\r第二行\n第三行"));
    }

    public static byte[] Generate(bool isXibao, string text)
    {
        SKTypeface face = SKTypeface.FromFile("HarmonyOS.ttf");
        SKFont font = new SKFont(face, 100);
        using SKSurface? surface = SKSurface.Create(new SKImageInfo(1024, 768));
        SKCanvas? canvas = surface.Canvas;
        SKPaint paint = new SKPaint()
        {
            IsAntialias = true,
            Color = isXibao ? new SKColor(255, 10, 10) : new SKColor(0, 5, 0)
        };
        using SKBitmap halo = SKBitmap.Decode(isXibao ? "xibao.jpg" : "beibao.jpg");
        canvas.DrawBitmap(halo, 0, 0, paint);
        DrawCenteredText(canvas, text, 1024, 512, 384, font, paint);
        using SKData? output = surface.Snapshot().Encode();
        return output.ToArray();
    }

    public static void DrawCenteredText(SKCanvas canvas, string text, float width, float x, float y, SKFont font, SKPaint paint)
    {
        List<string> lines = [];
        while (!string.IsNullOrEmpty(text))
        {
            if (text.StartsWith('\r') || text.StartsWith('\n'))
            {
                text = text[1..];
                continue;
            }

            int breakIndex = font.BreakText(text, width, out _);
            int rIndex = text.IndexOf('\r');
            int nIndex = text.IndexOf('\n');
            rIndex = rIndex == -1 ? int.MaxValue : rIndex;
            nIndex = nIndex == -1 ? int.MaxValue : nIndex;
            int feedIndex = Math.Min(rIndex, nIndex);

            if (feedIndex < breakIndex)
            {
                lines.Add(text[..feedIndex]);
                text = text[(feedIndex + 1)..];
            }
            else
            {
                lines.Add(text[..breakIndex]);
                text = text[breakIndex..];
            }
        }

        float height = lines.Count * font.Spacing;
        float baselineY = y - height / 2 - font.Metrics.Ascent;
        foreach (string line in lines)
        {
            canvas.DrawText(line, x, baselineY, SKTextAlign.Center, font, paint);
            baselineY += font.Spacing;
        }
    }
}