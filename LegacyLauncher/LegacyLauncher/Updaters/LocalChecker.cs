using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LegacyLauncher.Updaters
{
	public class LocalChecker : UpdateChecker
	{
		public override VersionInfo Check()
		{
			var version = new VersionInfo();

			WebClient webClient = new WebClient();
			var data = webClient.DownloadString("http://localhost:25567/Version.txt");
			Logger.Log(data);
			var parts = data.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
			version.ClientVersion = Int32.Parse(parts[0]);
			version.ClientLink = "http://localhost:25567/Update.zip";

			version.LauncherVersion = Int32.Parse(parts[1]);
			version.LauncherLink = "http://localhost:25567/LegacyLauncher.exe";

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
