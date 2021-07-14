using GalaSoft.MvvmLight;
using Microsoft.VisualBasic;
using System;
using System.Windows.Media;

namespace SldWorksLookup.ViewModel
{
    public class ColorToIntWindowViewModel : ViewModelBase
    {
        private Color? _selectedColor;
        private int _result;

        public Color? SelectedColor
        {
            get => _selectedColor; set
            {
                Set(ref _selectedColor, value);
                if (value != null)
                {
                    Result = FromColorToInt(value.Value);
                }
            }
        }

        public static int FromColorToInt(Color color)
        {
            return Information.RGB(color.R, color.G, color.B);
        }

        public int Result { get => _result; set => Set(ref _result ,value); }
    }
}
