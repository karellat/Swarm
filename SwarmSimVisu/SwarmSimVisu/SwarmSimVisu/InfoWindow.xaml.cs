using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SwarmSimVisu
{
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        private MainWindow parentWindow; 
        public InfoWindow(string text,MainWindow parentWindow)
        {
            InitializeComponent();
            textBlock.Text = text;
            this.FitSize();
            this.parentWindow = parentWindow;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FitSize()
        {
            if (this.Parent is FrameworkElement parent)
            {
                var targetWidthSize = this.FontSize;
                var targetHeightSize = this.FontSize;

                var maxWidth = double.IsInfinity(this.MaxWidth) ? parent.ActualWidth : this.MaxWidth;
                var maxHeight = double.IsInfinity(this.MaxHeight) ? parent.ActualHeight : this.MaxHeight;

                if (this.ActualWidth > maxWidth)
                {
                    targetWidthSize = (double)(this.FontSize * (maxWidth / (this.ActualWidth )));
                }

                if (this.ActualHeight > maxHeight)
                {
                    var ratio = maxHeight / (this.ActualHeight);

                    // Normalize due to Height miscalculation. We do it step by step repeatedly until the requested height is reached. Once the fontsize is changed, this event is re-raised
                    // And the ActualHeight is lowered a bit more until it doesnt enter the enclosing If block.
                    ratio = (1 - ratio > 0.04) ? Math.Sqrt(ratio) : ratio;

                    targetHeightSize = (double)(this.FontSize * ratio);
                }

                this.FontSize = Math.Min(targetWidthSize, targetHeightSize);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Dispatcher.Invoke(() => { parentWindow.RemoveInfo(this); });
            base.OnClosing(e);
        }
    }
}
