using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LegacyLauncher.Updaters
{
	class Downloader
	{
		public static void Download(VersionInfo version)
		{
			if (version.LauncherVersion > MainWindow.VERSION)
			{
				WebClient webClient = new WebClient();
				var statusForm = new Forms.Download();
				statusForm.Show();
				//Stopwatch sw = new Stopwatch();
				//long lastBytes = 0;

				webClient.DownloadProgressChanged += (dlSender, args) =>
				{
					int value = args.ProgressPercentage;
					if (statusForm.PB_Progress.Value != args.ProgressPercentage)
					{
						statusForm.TXT_Percentage.Text = (int)value + "%";
						statusForm.PB_Progress.Value = (int)value;

						//var loaded = args.BytesReceived - lastBytes;
						//decimal perSecBytes = loaded / sw.ElapsedMilliseconds * 1000;
						//statusForm.TXT_Speed.Text = Math.Round(perSecBytes / 1024 / 1024, 1) + " МБ/с";

						//lastBytes = args.BytesReceived;
						//sw.Restart();
					}
				};

				webClient.DownloadFileCompleted += (dlSender, args) =>
				{
					statusForm.Close();
					Utils.InstallLauncher(version);
					webClient.Dispose();
				};

				var url = version.LauncherLink;
				webClient.DownloadFileAsync(new Uri(url), @"tmp_Launcher.exe");
				//sw.Start();
			}
			else if (version.ClientVersion != 0)
			{
				WebClient webClient = new WebClient();
				var statusForm = new Forms.Download();
				statusForm.Show();
				Stopwatch sw = new Stopwatch();

				long lastBytes = 0;

				webClient.DownloadProgressChanged += (dlSender, args) =>
				{
					int value = args.ProgressPercentage;
					if (statusForm.PB_Progress.Value != args.ProgressPercentage)
					{
						statusForm.TXT_Percentage.Text = (int)value + "%";
						statusForm.PB_Progress.Value = (int)value;

						var loaded = args.BytesReceived - lastBytes;
						decimal perSecBytes = loaded / sw.ElapsedMilliseconds * 1000;
						statusForm.TXT_Speed.Text = Math.Round(perSecBytes / 1024 / 1024, 1) + " МБ/с";

						lastBytes = args.BytesReceived;
						sw.Restart();
					}
				};

				webClient.DownloadFileCompleted += (dlSender, args) =>
				{
					statusForm.Close();

					Installer.Install(version);
					webClient.Dispose();
				};

				var url = version.ClientLink;
				webClient.DownloadFileAsync(new Uri(url), @"Update.zip");
				sw.Start();
			}

			
			
		}
	}
}
