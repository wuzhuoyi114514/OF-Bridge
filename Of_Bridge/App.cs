using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;

namespace Of_Bridge
{
	public class App : Application
	{
		private bool isSupportDarkMode = Environment.OSVersion.Version.Major == 10 && Environment.OSVersion.Version.Build >= 17763;

		private bool _contentLoaded;

		public App()
		{
			if (isSupportDarkMode)
			{
				UxTheme.AllowDarkModeForApp(allow: true);
				UxTheme.ShouldSystemUseDarkMode();
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "6.0.9.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				base.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
				Uri resourceLocator = new Uri("/Of Bridge;component/app.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		[STAThread]
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "6.0.9.0")]
		public static void Main()
		{
			App app = new App();
			app.InitializeComponent();
			app.Run();
		}
	}
}
