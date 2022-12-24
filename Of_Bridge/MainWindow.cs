using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Markup;
using ModernWpf;
using ModernWpf.Controls;

namespace Of_Bridge
{
	public class MainWindow : Window, IComponentConnector
	{
		private bool isSupportDarkMode = Environment.OSVersion.Version.Major == 10 && Environment.OSVersion.Version.Build >= 17763;

		private bool isSupportDarkMode2 = Environment.OSVersion.Version.Major == 10 && Environment.OSVersion.Version.Build >= 18362;

		internal TextBox OfBridge_IPInput;

		internal TextBlock OfBridge_Error;

		internal NumberBox OfApp_Number;

		internal TextBox Of_Log;

		private bool _contentLoaded;

		private bool AppState { get; set; }

		private Socket? serverSocket { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			Append("应用开启");
		}

		protected override void OnContentRendered(EventArgs e)
		{
			base.OnContentRendered(e);
			Window_ActualThemeChanged(this, new RoutedEventArgs());
		}

		private void Window_ActualThemeChanged(object sender, RoutedEventArgs e)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Invalid comparison between Unknown and I4
			IntPtr handle = new WindowInteropHelper(this).Handle;
			if (isSupportDarkMode)
			{
				if ((int)ThemeManager.GetActualTheme((FrameworkElement)this) == 2)
				{
					UxTheme.DwmSetWindowAttribute(handle, UxTheme.DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref UxTheme.TrueValue, 4);
				}
				else
				{
					UxTheme.DwmSetWindowAttribute(handle, UxTheme.DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref UxTheme.FalseValue, 4);
				}
			}
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			string text = ((Button)sender).Content as string;
			if (text != null && text == "开始联机")
			{
				((Button)sender).IsEnabled = false;
				string[] _temple_1 = OfBridge_IPInput.Text.Split(':');
				OfBridge_Error.Text = string.Empty;
				try
				{
					IPEndPoint serverIp = new IPEndPoint((await Dns.GetHostAddressesAsync(_temple_1.First())).First(), Convert.ToInt32(_temple_1[1]));
					IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 0);
					serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
					serverSocket!.Bind(localEP);
					serverSocket!.Listen(100);
					AppState = true;
					new Thread((ThreadStart)delegate
					{
						MotdBroadCaster((serverSocket!.LocalEndPoint as IPEndPoint).Port);
					}).Start();
					new Thread((ThreadStart)delegate
					{
						Socket client = null;
						Socket server = null;
						try
						{
							while (AppState)
							{
								client = serverSocket!.Accept();
								server = new Socket(SocketType.Stream, ProtocolType.Tcp);
								server.Connect(serverIp);
								Append($"获得连接!!! {client.LocalEndPoint as IPEndPoint}");
								new Thread((ThreadStart)delegate
								{
									Forward(client, server);
								}).Start();
								new Thread((ThreadStart)delegate
								{
									Forward(server, client);
								}).Start();
							}
						}
						catch (Exception ex3)
						{
							try
							{
								client?.Disconnect(reuseSocket: false);
								server?.Close();
							}
							catch (Exception ex2)
							{
								Append(ex2.ToString());
							}
							AppState = false;
							Append(ex3.ToString());
						}
					}).Start();
					((Button)sender).Content = "关闭联机";
					((Button)sender).IsEnabled = true;
				}
				catch (Exception ex)
				{
					OfBridge_Error.Text = "输入的格式不正确。";
					Append(ex.ToString());
					((Button)sender).IsEnabled = true;
				}
			}
			else
			{
				serverSocket?.Close();
				((Button)sender).Content = "开始联机";
			}
		}

		private void Append(string str)
		{
			string str2 = str;
			Application.Current?.Dispatcher.Invoke(delegate
			{
				Of_Log.AppendText($"\n[{DateTimeOffset.Now}] {str2}");
			});
		}

		private void MotdBroadCaster(int port)
		{
			UdpClient udpClient = new UdpClient("224.0.2.60", 4445);
			byte[] bytes = Encoding.UTF8.GetBytes($"[MOTD]§eOf Bridge 2.0 || 双击进入[/MOTD][AD]{port}[/AD]");
			Append("联机广播已开启!!!");
			while (AppState)
			{
				udpClient.Send(bytes, bytes.Length);
			}
		}

		private void Forward(Socket s, Socket c)
		{
			int num = Application.Current?.Dispatcher.Invoke(() => (int)OfApp_Number.get_Value()) ?? 1536;
			try
			{
				byte[] array = new byte[num];
				while (AppState)
				{
					if (AppState)
					{
						int num2 = s.Receive(array, 0, array.Length, SocketFlags.None);
						if (num2 <= 0)
						{
							break;
						}
						c.Send(array, 0, num2, SocketFlags.None);
					}
				}
			}
			catch (Exception ex)
			{
				Append(ex.ToString());
			}
			finally
			{
				c.Close();
				s.Close();
			}
		}

		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			Process.Start((sender as Hyperlink).NavigateUri.ToString());
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "6.0.9.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/Of Bridge;component/mainwindow.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "6.0.9.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Expected O, but got Unknown
			switch (connectionId)
			{
			case 1:
				((MainWindow)target).AddHandler(ThemeManager.ActualThemeChangedEvent, new RoutedEventHandler(Window_ActualThemeChanged));
				break;
			case 2:
				OfBridge_IPInput = (TextBox)target;
				break;
			case 3:
				((Button)target).Click += Button_Click;
				break;
			case 4:
				OfBridge_Error = (TextBlock)target;
				break;
			case 5:
				((Hyperlink)target).Click += Hyperlink_Click;
				break;
			case 6:
				((Hyperlink)target).Click += Hyperlink_Click;
				break;
			case 7:
				OfApp_Number = (NumberBox)target;
				break;
			case 8:
				Of_Log = (TextBox)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
