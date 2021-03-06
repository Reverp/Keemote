﻿using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Keemote
{
	public sealed class KeemoteExt : Plugin
	{
		private IPluginHost pluginHost = null;
		private ToolStripMenuItem toolstripMenuItem = null;

		public override bool Initialize(IPluginHost host)
		{
			pluginHost = host;

			ContextMenuStrip entryMenuStrip = pluginHost.MainWindow.EntryContextMenu;

			entryMenuStrip.Opened += new EventHandler(entryMenu_Opened);

			var toolstripSeperator = new ToolStripSeparator();
			entryMenuStrip.Items.Add(toolstripSeperator);

			// Add Remote Desktop Connection ToolStripMenuIem
			toolstripMenuItem = new ToolStripMenuItem();
			toolstripMenuItem.Image = Properties.Resources.RemoteDesktopIcon;
			toolstripMenuItem.Text = "Start remote desktop connection...";
			toolstripMenuItem.Click += new EventHandler(this.StartRemoteDesktopConnection);
			entryMenuStrip.Items.Add(toolstripMenuItem);

			return true;
		}

		public override void Terminate()
		{
			// TODO: Remove the correct items from the contextmenu
		}

		void entryMenu_Opened(object sender, EventArgs e)
		{
			if (toolstripMenuItem != null && pluginHost.MainWindow.GetSelectedEntriesCount() > 0)
			{
				toolstripMenuItem.Enabled = true;
			}
			else if (toolstripMenuItem == null)
			{
				return;
			}
			else
			{
				toolstripMenuItem.Enabled = false;
			}
		}


		private void StartRemoteDesktopConnection(object sender, EventArgs e)
		{
			//Retrieve selected entry in order to get values
			PwEntry entry = pluginHost.MainWindow.GetSelectedEntry(false);
			ProtectedString protectedUsername = entry.Strings.Get("UserName");
			ProtectedString protectedPassword = entry.Strings.Get("Password");
			ProtectedString protectedUrl = entry.Strings.Get("URL");

			// TODO: Remove credentials from cmdkey after a period of time
			// Run cmdkey in order to 'temporarily' save credentials which mstsc can use
			Process cmdKeyProcess = new Process();
			cmdKeyProcess.StartInfo.FileName = "cmdkey.exe";
			cmdKeyProcess.StartInfo.Arguments = $"/generic:TERMSRV/{protectedUrl.ReadString()} /user:\"{protectedUsername.ReadString()}\" /pass:\"{protectedPassword.ReadString()}\"";
			cmdKeyProcess.Start();

			Process remoteDesktopProcess = new Process();
			remoteDesktopProcess.StartInfo.FileName = "mstsc.exe";
			remoteDesktopProcess.StartInfo.Arguments = $"/v:{protectedUrl.ReadString()}";
			remoteDesktopProcess.Start();
		}
	}
}