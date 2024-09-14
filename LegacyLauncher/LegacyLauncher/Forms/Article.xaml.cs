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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LegacyLauncher.Forms
{
	/// <summary>
	/// Логика взаимодействия для Article.xaml
	/// </summary>
	public partial class Article : UserControl
	{
		public Article(string data)
		{
			// Automatically resize height relative to content
			string[] parts = data.Split(new[] { ">>>" }, StringSplitOptions.RemoveEmptyEntries);

			var name = parts[0];
			var text = parts[1];

			InitializeComponent();


			MaxWidth = 385;
			MinWidth = MaxWidth;

			TXT_Name.Text = name.Trim();
			TXT_Text.Text = text.Trim();

			//TXT_Name.MaxWidth = 380;
			//TXT_Text.MaxWidth = 380;

			TXT_Name.Measure(new Size(443.187, 10000000));
			TXT_Text.Measure(new Size(443.187, 10000000));

			TXT_Text.Height = TXT_Text.DesiredSize.Height;
			Height = TXT_Text.DesiredSize.Height + 60;
		}
	}
}
