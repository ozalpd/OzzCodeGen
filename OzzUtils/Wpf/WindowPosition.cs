using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace OzzUtils.Wpf
{
    public class WindowPosition
    {
        public double Top { get; set; }
        public double Left { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        [XmlIgnore]
        public bool IsOnPrimaryScreen { get; private set; }


        public void GetWindowPositions(Window window)
        {
            Top = window.Top;
            Left = window.Left;
            Height = window.Height;
            Width = window.Width;
        }

        public void SetWindowPositions(Window window)
        {
            double taskBarHeight = (SystemParameters.PrimaryScreenHeight - SystemParameters.MaximizedPrimaryScreenHeight) + 10;
            double maxWinHeight = SystemParameters.VirtualScreenHeight - taskBarHeight;
            double maxWinWidth = SystemParameters.VirtualScreenWidth;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenLeft = SystemParameters.VirtualScreenLeft;

            Rect windowRectangle = new Rect(Left, Top, Width, Height);
            var minVisible = new Size(10.0, 10.0);

            if (Screen.AllScreens.Length > 1)//TODO:Fix this => only works when screen pixels are equals to WPF pixels
            {
                foreach (var screen in Screen.AllScreens)
                {
                    var workingArea = new Rect(screen.WorkingArea.Left,
                                               screen.WorkingArea.Top,
                                               screen.WorkingArea.Width,
                                               screen.WorkingArea.Height);
                    var intersection = Rect.Intersect(windowRectangle, workingArea);
                    if (intersection.Width >= minVisible.Width && intersection.Height >= minVisible.Height)
                    {
                        var wArea = screen.WorkingArea;
                        maxWinHeight = wArea.Height;
                        maxWinWidth = wArea.Width;
                        screenTop = wArea.Top;
                        screenLeft = wArea.Left;
                        IsOnPrimaryScreen = screen.Primary;

                        break;
                    }
                }
            }

            if (Top < screenTop || Top + window.MinHeight >= screenTop + maxWinHeight || Height > maxWinHeight)
                Top = screenTop;

            if (Left < screenLeft || Left + window.MinWidth >= screenLeft + maxWinWidth || Width > maxWinWidth)
                Left = screenLeft;

            if (Height > maxWinHeight)
            {
                Top = screenTop;
                Height = maxWinHeight;
            }

            if (Width > maxWinWidth)
            {
                Left = screenLeft;
                Width = maxWinWidth;
            }

            if ((Left + Width) > screenLeft + maxWinWidth)
                Width = 0;

            if ((Top + Height) > screenTop + maxWinHeight)
                Height = 0;

            if (Width > 0)
                window.Width = Width;

            if (Height > 0)
                window.Height = Height;

            window.Top = Top;
            window.Left = Left;
        }
    }
}
