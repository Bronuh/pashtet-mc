using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Updaters
{
	public class BronuhChecker : UpdateChecker
	{
		public override VersionInfo Check()
		{
			var version = new VersionInfo();

			WebClient webClient = new WebClient();
			var data = webClient.DownloadString("http://bronuh.zapto.org:25567/Version.txt");
			Logger.Log(data);
			var parts = data.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			int partsCount = parts.Length;

			version.ClientVersion = Int32.Parse(parts[0]);
			version.ClientLink = "http://bronuh.zapto.org:25567/Update.zip";
			if (parts.Length > 1)
			{
				version.LauncherVersion = Int32.Parse(parts[1]);
				version.LauncherLink = "http://bronuh.zapto.org:25567/Launcher.exe";
			}

			webClient.Dispose();
			return version;
		}

		public override void Update()
		{
			VersionInfo version = Check();
			Downloader.Download(version);
		}
	}
}
