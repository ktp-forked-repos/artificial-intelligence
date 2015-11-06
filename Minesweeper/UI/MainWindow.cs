using System;
using System.Windows.Forms;
using Game.Core.Interfaces;

namespace Minesweeper.UI
{
  public sealed partial class MainWindow 
    : Form, IDrawingWindow
  {
    public MainWindow(int processId)
    {
      InitializeComponent();

      Text = string.Format("Minesweeper PID {0}", processId);

      // force the panel to get focus so it will receive mouse scroll events
      drawingPanel.MouseHover += (sender, args) => drawingPanel.Focus();
    }

    public IntPtr DrawingPanelHandle
    {
      get { return drawingPanel.Handle; }
    }
  }
}
