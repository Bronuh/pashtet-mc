using System;
using System.Collections.Generic;
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

namespace Launcher.Forms
{
	/// <summary>
	/// Логика взаимодействия для Download.xaml
	/// </summary>
	public partial class Download : Window
	{
		public Download()
		{
			InitializeComponent();
		}

		private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			// TXT_Percentage.Text = e.NewValue+"%";
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (PB_Progress.Value<99)
			{
				e.Cancel = true;
			}
			
		}
	}
}
