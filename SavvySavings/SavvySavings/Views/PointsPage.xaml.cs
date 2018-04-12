using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SkiaSharp;
using SkiaSharp.Views.Forms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SavvySavings.ViewModels;

namespace SavvySavings.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PointsPage : ContentPage
	{
        AccountViewModel viewModel;
        static float startP;
        static float scrollHeight = 0;
        static double sPoints = 0;
        Boolean runScroller = false;
        SKRect hitBounds;
        List<SKRect> hitRects;
        float animationIndex = 0;
        Boolean shouldAnimate = false;
        Boolean animateClick = false;
        float animationPositon = 0;
        float animationTime;
        float textAnimationStart;
        List<float> pointsList;
        List<string> pointsNumbers;
        List<string> pointsLevels;
        List<string> pointsText;

        public PointsPage ()
		{
			InitializeComponent ();
            BindingContext = viewModel = new AccountViewModel();

            hitRects = new List<SKRect>();
            pointsList = new List<float>() { -6,-24,-54,-96,-150,-216,-294,-384,-486,-600 };
            pointsNumbers = new List<string>() { "100", "400", "900", "1600", "2500", "3600", "4900", "6400", "8100", "10000" };
            pointsLevels = viewModel.SPLevels;
            pointsText = viewModel.SPText;

            animationTime = 20;
            textAnimationStart = -50;
            var tmpAnimationDiv = 0;
            Device.StartTimer(TimeSpan.FromSeconds(1f / 60), () =>
            {
                canvasView.InvalidateSurface();
                if (animateClick)
                {
                    animationPositon = tmpAnimationDiv;
                    tmpAnimationDiv += 1;
                    if (tmpAnimationDiv == animationTime)
                    {
                        animateClick = false;
                        //animationPositon = animationWidth;
                        tmpAnimationDiv = 0;
                    }
                }
                if (runScroller)
                {
                    var scrollStop = canvasView.Height * 3 * (animationIndex / 10); //(Math.Sqrt(sPoints) / 100);
                    if (scrollHeight < scrollStop)
                    {
                        if (scrollHeight > scrollStop * 0.8)
                            scrollHeight += 4;
                        else if (scrollHeight > scrollStop * 0.6)
                            scrollHeight += 5;
                        else if (scrollHeight > scrollStop * 0.4)
                            scrollHeight += 6;
                        else if (scrollHeight > scrollStop * 0.2)
                            scrollHeight += 7;
                        else
                            scrollHeight += 8;
                    } else
                    {
                        runScroller = false;
                    }
                }
                return true;
            });
        }

        void Redeem_Clicked(object sender, EventArgs e)
        {
            Console.WriteLine("Redeem Clicked");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.CheckIns.Count == 0)
                viewModel.LoadCheckInsCommand.Execute(null);
        }

        static SColors sColors = new SColors();
        SKPaint circlePaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = sColors.Primary
        };
        SKPaint spPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 28,
            IsAntialias = true
        };
        SKPaint titlePaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 24,
            IsAntialias = true
        };
        SKPaint nullPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = sColors.Grey
        };
        SKPaint thermLine = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = 4,
            IsAntialias = true
        };
        SKPaint levelsPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 12,
            TextAlign = SKTextAlign.Right,
            IsAntialias = true
        };
        SKPaint textPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 12,
            TextAlign = SKTextAlign.Center
        };
        SKPaint pointsLevelText = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 14,
            TextAlign = SKTextAlign.Right,
            IsAntialias = true
        };
        SKPaint titleUnderline = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 1
        };
        SKPaint hitRectPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Transparent
        };

        private void OnCanvasViewPaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            int width = info.Width;
            int height = info.Height;
            int canvasHeight = height * 2;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            //Scaling Parameters
            float scalar = height / 200f;
            float quadWidth = width / scalar / 2;
            float quad3width = quadWidth / 3;

            // Manage Overscroll
            if (scrollHeight < 0)
                scrollHeight = 0;
            if (scrollHeight > canvasHeight)
                scrollHeight = canvasHeight;

            // Set Transformations
            canvas.Translate(width / 2, height + scrollHeight);
            canvas.Scale(scalar);

            canvas.Clear(sColors.Teal);

            var pointsLevel = Math.Sqrt(sPoints) / 10;
            float pointsHeight = (float)(600 * (pointsLevel / 10));
            float nullHeight = 600 - pointsHeight;
            float floorPointsLevel = (float)Math.Floor(pointsLevel);
            float pointsTextHeight = -pointsHeight - 18;
            if (animationIndex == floorPointsLevel + 1)
                pointsTextHeight += 50;
            if (sPoints != viewModel.SPoints)
            {
                sPoints = viewModel.SPoints;
                animationIndex = (float)Math.Floor(Math.Sqrt(sPoints) / 10);
                runScroller = true;
                animateClick = true;
            }

            


            var pointsRect = new SKRect(quad3width, 0, quad3width, -pointsHeight);
            //canvas.DrawRect(quad3width, 0, quad3width, -pointsHeight, circlePaint);
            canvas.DrawText($"{sPoints} SP", -2 * quad3width, pointsTextHeight, spPaint);
            canvas.DrawRect(pointsRect, circlePaint);
            hitBounds = new SKRect(quad3width, -canvasHeight, quad3width * 2, 0);
            if (pointsHeight < 600)
            {
                //canvas.DrawRect(quad3width, -pointsHeight, quad3width, -nullHeight, nullPaint);
                var nullRect = new SKRect(quad3width, -pointsHeight, quad3width, -nullHeight);
                canvas.DrawRect(nullRect, nullPaint);
            }
            canvas.DrawCircle(quadWidth / 2, 0, quad3width, circlePaint);

            //float segment1 = (float)Math.Sqrt(Math.Pow(quad3width, 2) - 36);
            //float segment2 = (float)Math.Sqrt(Math.Pow(quad3width, 2) - 576);

            //canvas.DrawLine((quadWidth / 2) - segment1, -6, (quadWidth / 2) + segment1, -6, thermLine);
            //canvas.DrawLine((quadWidth / 2) - segment2, -24, (quadWidth / 2) + segment2, -24, thermLine);
            hitRects.Clear();
            for (var i = 0; i < pointsList.Count; i++)
            {
                hitRects.Add(new SKRect(quad3width, -60 * (i + 2) + 3, quad3width * 2, -60 * (i + 1) + 3));
                canvas.DrawLine(quad3width, -60 * (i + 1) + 3, quad3width * 2, -60 * (i + 1) + 3, thermLine);
                canvas.DrawText(pointsNumbers[i], quadWidth - 5, -60 * (i + 1) + 9, levelsPaint);
                if(animationIndex - 1 != i)
                canvas.DrawText(pointsLevels[i], quad3width - 5, -60 * (i + 1) - 21, pointsLevelText);
                //TODO: ADD colors
            };
            //Here are the tests
            int textIndex = (int)animationIndex - 1;
            var lineAnimationDiv = -5 * (quad3width / 2);
            var lineAnimation = (lineAnimationDiv / animationTime) * animationPositon;
            var titleAnimationDiv = -5 * (quad3width / 2);
            titlePaint.Color = titlePaint.Color.WithAlpha((byte)(255 * (animationPositon / animationTime)));
            var titleAnimation = titleAnimationDiv - (textAnimationStart - (textAnimationStart * (animationPositon / animationTime)));
            var textAnimationDiv = -quad3width;
            var textAnimation = textAnimationDiv - (textAnimationStart - (textAnimationStart * (animationPositon / animationTime)));
            textPaint.Color = textPaint.Color.WithAlpha((byte)(255 * (animationPositon / animationTime)));

            if (animationPositon > -1 && textIndex > -1)
            {
                canvas.DrawLine(lineAnimation, -60 * animationIndex - 18, quad3width / 2, -60 * animationIndex - 18, titleUnderline);
                canvas.DrawText(pointsLevels[textIndex], titleAnimation, -60 * animationIndex - 24, titlePaint);
                var splitPointsText = SplitLine(160, pointsText[textIndex]);
                for(var i = 0; i < splitPointsText.Length; i++)
                {
                    canvas.DrawText(splitPointsText[i], textAnimation, -60 * animationIndex + (12 * i), textPaint);
                }
            }
            
            //SKPath path = SKPath.ParseSvgPathData("M0 0 V100 H100 V-100 H0 Z");
        }

        private string[] SplitLine(float maxWidth, string text)
        {
            var result = new List<string>();
            var spaceWidth = pointsLevelText.MeasureText(" ");
            var words = text.Split(new[] { " " }, StringSplitOptions.None);

            var line = new StringBuilder();
            float width = 0;
            foreach (var word in words)
            {
                var wordWidth = pointsLevelText.MeasureText(word);
                var wordWithSpaceWidth = wordWidth + spaceWidth;
                var wordWithSpace = word + " ";

                if (width + wordWidth > maxWidth)
                {
                    result.Add(line.ToString());
                    line = new StringBuilder(wordWithSpace);
                    width = wordWithSpaceWidth;
                }
                else
                {
                    line.Append(wordWithSpace);
                    width += wordWithSpaceWidth;
                }
            }

            result.Add(line.ToString());

            return result.ToArray();
        }

        public class SColors
        {
            private Color colorPrimary = (Color)App.Current.Resources["Primary"];
            public SKColor Primary
            {
                get { return colorPrimary.ToSKColor(); }
            }
            private SKColor teal = Color.FromHex("0B877D").ToSKColor();
            public SKColor Teal
            {
                get { return teal; }
            }
            private SKColor grey = Color.FromHex("e3edee").ToSKColor();
            public SKColor Grey
            {
                get { return grey; }
            }
        }

        private void CanvasView_Touch(object sender, SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    runScroller = false;
                    shouldAnimate = true;
                    startP = e.Location.Y - scrollHeight;
                    break;
                case SKTouchAction.Moved:
                    if (e.InContact)
                    {
                        shouldAnimate = false;
                        scrollHeight = e.Location.Y - startP;
                    }
                    break;
                case SKTouchAction.Released:
                case SKTouchAction.Cancelled:
                    if (shouldAnimate)
                    HitTest(e.Location);
                    break;
            }

            // we have handled these events
            e.Handled = true;

            // update the UI
            ((SKCanvasView)sender).InvalidateSurface();
        }

        public void HitTest(SKPoint location)
        {
            var scalar = canvasView.CanvasSize.Height / 200f;
            var halfWidth = canvasView.CanvasSize.Width / scalar / 2;
            var height = canvasView.CanvasSize.Height / scalar;

            float newX = (float)((location.X / scalar) - (halfWidth));
            float newY = (float)((location.Y / scalar) - height - (scrollHeight / scalar));
            SKPoint hitPoint = new SKPoint(newX,newY);

            if(hitBounds.Contains(hitPoint))
            {
                foreach(SKRect rect in hitRects)
                {
                    if (rect.Contains(hitPoint))
                    {
                        var hitIndex = hitRects.IndexOf(rect) + 1;
                        if (hitIndex != animationIndex)
                        {
                            animationIndex = hitIndex;
                            animateClick = true;
                        }
                    }
                }
            }
        }
    }
}