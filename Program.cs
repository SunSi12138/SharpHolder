using SkiaSharp;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.SetMinimumLevel(LogLevel.None);
var app = builder.Build();

var random = new Random();
var imageInfo = new SKImageInfo(1920,1080);

var textPaint = new SKPaint()
{
    Typeface = SKTypeface.FromFile("AlibabaPuHuiTi-3-55-Regular.woff2")
};

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        if (exception is BadHttpRequestException badHttpRequestException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync(badHttpRequestException.Message);
        }
    });
});
app.MapGet("/placeholder", GetHandler);

app.Run();



SKColor GetComplementaryColor (SKColor color) {
    color.ToHsl(out var h, out var s, out var l);
    h = (h + 180) % 360;
    l = l > 50 ? l - 50 : l + 50;
    return SKColor.FromHsl(h, s, l);
}

SKColor getRandomColor() => SKColor.FromHsl(random.Next(0, 360), 50, 80);




async Task GetHandler (HttpContext httpContext,string? b,string? f,string? t,string? e,int? d,bool a=true,int q=100,int h=1080, int w=1920)
{


    if (w <= 0 || h <= 0 || q<0 || q>100 || d<0)
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsync("Invalid parameters");
        return;
    }

    var trueImageInfo = imageInfo.WithSize(w, h);

    using var surface = SKSurface.Create(trueImageInfo);
    var canvas = surface.Canvas;

    var bgColor = string.IsNullOrEmpty(b) || !SKColor.TryParse(b, out var color) ? getRandomColor() : color;
    var textColor = string.IsNullOrEmpty(f) || !SKColor.TryParse(f, out var color1) ? GetComplementaryColor(bgColor) : color1;
    var trueText = t ?? $"{w}x{h}";

    if (bgColor.Alpha != 255 || textColor.Alpha != 255)
    {
        trueImageInfo.AlphaType = SKAlphaType.Premul;
        trueImageInfo.ColorType = SKColorType.Rgba8888;
    }
    else
    {
        trueImageInfo.AlphaType = SKAlphaType.Opaque;
        trueImageInfo.ColorType = SKColorType.Rgb888x;
    }

    canvas.Clear(bgColor);

    textPaint.IsAntialias = a;
    textPaint.Color = textColor;
    textPaint.TextSize = w / trueText.Length;

    var textBounds = new SKRect();
    textPaint.MeasureText(trueText, ref textBounds);
    var x = (w - textBounds.Width) / 2;
    var y = (h + textBounds.Height) / 2;
    canvas.DrawText(trueText, x, y, textPaint);
    var betterEncode = trueImageInfo.AlphaType == SKAlphaType.Opaque ? SKEncodedImageFormat.Jpeg : SKEncodedImageFormat.Png;
    var encode = e is null ? betterEncode : Enum.TryParse<SKEncodedImageFormat>(e, true, out var format) ? format : betterEncode;

    using var image = surface.Snapshot();
    using var data = image.Encode(encode, q);
    
    var imageData = data.ToArray();

    httpContext.Response.ContentType = $"image/{encode.ToString().ToLower()}";
    httpContext.Response.Headers.CacheControl = "no-cache no-store must-revalidate";
    httpContext.Response.Headers.Expires = "-1";
    
    if (d.HasValue)
    {
        int totalDelay = d.Value;
        int idealChunkTime = 10;
        int chunks = totalDelay / idealChunkTime;
        chunks = Math.Max(chunks, 1);

        int chunkSize = imageData.Length / chunks;
        int delayPerChunk = totalDelay / chunks;
        for (int i = 0; i < chunks; i++)
        {
            int offset = i * chunkSize;
            int length = (i < chunks - 1) ? chunkSize : imageData.Length - offset; 

            await httpContext.Response.Body.WriteAsync(imageData.AsMemory(offset, length));
            await httpContext.Response.Body.FlushAsync();
            await Task.Delay(delayPerChunk);
        }
    }
    else
    {
        await httpContext.Response.Body.WriteAsync(imageData);
    }
}