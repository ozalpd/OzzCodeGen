using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace OzzCodeGen.Wpf.Models
{
    public class WindowPosition
    {
        public double Top { get; set; }
        public double Left { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        [XmlIgnore]
        public double ScreenHeigth
        {
            get
            {
                if (!_screenHeigth.HasValue)
                {
                    _screenHeigth = SystemParameters.VirtualScreenHeight;
                }
                return _screenHeigth.Value;
            }
        }
        double? _screenHeigth;

        [XmlIgnore]
        public double ScreenWidth
        {
            get
            {
                if (!_screenWidth.HasValue)
                {
                    _screenWidth = SystemParameters.VirtualScreenWidth;
                }
                return _screenWidth.Value;
            }
        }
        double? _screenWidth;



        public void GetWindowPositions(Window window)
        {
            Top = window.Top;
            Left = window.Left;
            Height = window.Height;
            Width = window.Width;
        }

        public void SetWindowPositions(Window window)
        {
            double maxWinHeight = ScreenHeigth - 40;
            double screenTop = SystemParameters.VirtualScreenTop;

            if (Top < 0 | Top >= maxWinHeight | Height > maxWinHeight) Top = 0;
            if (Left < 0 | Left >= ScreenWidth | Width > ScreenWidth) Left = 0;
            if (Top == 0 & Height > maxWinHeight) Height = maxWinHeight;
            if (Left == 0 & Width > ScreenWidth) Width = ScreenWidth;

            if ((Left + Width) > ScreenWidth) Width = 0;
            if ((Top + Height) > maxWinHeight) Height = 0;

            if (Width > 0) window.Width = Width;
            if (Height > 0) window.Height = Height;

            var capH = SystemParameters.WindowCaptionHeight;

            window.Top = Top;
            window.Left = Left;
        }
    }
}
