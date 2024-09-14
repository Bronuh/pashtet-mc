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

namespace LegacyLauncher.Forms
{
	/// <summary>
	/// Логика взаимодействия для Mods.xaml
	/// </summary>
	public partial class Mods : Window
	{
		public Mods()
		{
			InitializeComponent();
			var mods = Mod.FoundMods;
			foreach (Mod mod in mods.Where(m => m.Clientside))
			{
				var item = new ListBoxItem();
				item.Tag = mod;
				item.Content = mod.Name;
				item.IsSelected = mod.Enabled;
				LST_Mods.Items.Add( item );

				item.Selected += (sender, e) => {
					mod.Enable();
				};
				item.Unselected += (sender, e) => {
					mod.Disable();
				};
				item.MouseEnter += (sender, e) =>
				{
					TXT_Description.Text = ((Mod)((ListBoxItem)sender).Tag).Description;
				};
			}
		}
	}
}
