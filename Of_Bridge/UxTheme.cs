using System;
using System.Runtime.InteropServices;

namespace Of_Bridge
{
	internal static class UxTheme
	{
		public enum PreferredAppMode
		{
			Default,
			AllowDark,
			ForceDark,
			ForceLight,
			Max
		}

		public enum DwmWindowAttribute : uint
		{
			DWMWA_USE_IMMERSIVE_DARK_MODE = 20u
		}

		public const string User32 = "user32.dll";

		public const string Uxtheme = "uxtheme.dll";

		public static int TrueValue = 1;

		public static int FalseValue = 0;

		[DllImport("uxtheme.dll", EntryPoint = "#104")]
		public static extern void RefreshImmersiveColorPolicyState();

		[DllImport("uxtheme.dll", EntryPoint = "#132")]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool ShouldAppsUseDarkMode();

		[DllImport("uxtheme.dll", EntryPoint = "#133")]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool AllowDarkModeForWindow(IntPtr hWnd, [MarshalAs(UnmanagedType.U1)] bool allow);

		[DllImport("uxtheme.dll", EntryPoint = "#135")]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool AllowDarkModeForApp([MarshalAs(UnmanagedType.U1)] bool allow);

		[DllImport("uxtheme.dll", EntryPoint = "#137")]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool IsDarkModeAllowedForWindow(IntPtr hWnd);

		[DllImport("uxtheme.dll", EntryPoint = "#135")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern PreferredAppMode SetPreferredAppMode(PreferredAppMode mode);

		[DllImport("uxtheme.dll", EntryPoint = "#138")]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool ShouldSystemUseDarkMode();

		[DllImport("uxtheme.dll", EntryPoint = "#139")]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool IsDarkModeAllowedForApp(IntPtr hWnd);

		[DllImport("dwmapi.dll")]
		public static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute);
	}
}
