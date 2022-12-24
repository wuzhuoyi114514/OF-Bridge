using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Costura
{
	[CompilerGenerated]
	internal static class AssemblyLoader
	{
		private static object nullCacheLock = new object();

		private static Dictionary<string, bool> nullCache = new Dictionary<string, bool>();

		private static Dictionary<string, string> assemblyNames = new Dictionary<string, string>();

		private static Dictionary<string, string> symbolNames = new Dictionary<string, string>();

		private static int isAttached;

		private static string CultureToString(CultureInfo culture)
		{
			if (culture == null)
			{
				return "";
			}
			return culture.Name;
		}

		private static Assembly ReadExistingAssembly(AssemblyName name)
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			Assembly[] assemblies = currentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				AssemblyName name2 = assembly.GetName();
				if (string.Equals(name2.Name, name.Name, StringComparison.InvariantCultureIgnoreCase) && string.Equals(CultureToString(name2.CultureInfo), CultureToString(name.CultureInfo), StringComparison.InvariantCultureIgnoreCase))
				{
					return assembly;
				}
			}
			return null;
		}

		private static void CopyTo(Stream source, Stream destination)
		{
			byte[] array = new byte[81920];
			int count;
			while ((count = source.Read(array, 0, array.Length)) != 0)
			{
				destination.Write(array, 0, count);
			}
		}

		private static Stream LoadStream(string fullName)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			if (fullName.EndsWith(".compressed"))
			{
				using (Stream stream = executingAssembly.GetManifestResourceStream(fullName))
				{
					using DeflateStream source = new DeflateStream(stream, CompressionMode.Decompress);
					MemoryStream memoryStream = new MemoryStream();
					CopyTo(source, memoryStream);
					memoryStream.Position = 0L;
					return memoryStream;
				}
			}
			return executingAssembly.GetManifestResourceStream(fullName);
		}

		private static Stream LoadStream(Dictionary<string, string> resourceNames, string name)
		{
			if (resourceNames.TryGetValue(name, out var value))
			{
				return LoadStream(value);
			}
			return null;
		}

		private static byte[] ReadStream(Stream stream)
		{
			byte[] array = new byte[stream.Length];
			stream.Read(array, 0, array.Length);
			return array;
		}

		private static Assembly ReadFromEmbeddedResources(Dictionary<string, string> assemblyNames, Dictionary<string, string> symbolNames, AssemblyName requestedAssemblyName)
		{
			string text = requestedAssemblyName.Name.ToLowerInvariant();
			if (requestedAssemblyName.CultureInfo != null && !string.IsNullOrEmpty(requestedAssemblyName.CultureInfo.Name))
			{
				text = requestedAssemblyName.CultureInfo.Name + "." + text;
			}
			byte[] rawAssembly;
			using (Stream stream = LoadStream(assemblyNames, text))
			{
				if (stream == null)
				{
					return null;
				}
				rawAssembly = ReadStream(stream);
			}
			using (Stream stream2 = LoadStream(symbolNames, text))
			{
				if (stream2 != null)
				{
					byte[] rawSymbolStore = ReadStream(stream2);
					return Assembly.Load(rawAssembly, rawSymbolStore);
				}
			}
			return Assembly.Load(rawAssembly);
		}

		public static Assembly ResolveAssembly(object sender, ResolveEventArgs e)
		{
			lock (nullCacheLock)
			{
				if (nullCache.ContainsKey(e.Name))
				{
					return null;
				}
			}
			AssemblyName assemblyName = new AssemblyName(e.Name);
			Assembly assembly = ReadExistingAssembly(assemblyName);
			if ((object)assembly != null)
			{
				return assembly;
			}
			assembly = ReadFromEmbeddedResources(assemblyNames, symbolNames, assemblyName);
			if ((object)assembly == null)
			{
				lock (nullCacheLock)
				{
					nullCache[e.Name] = true;
				}
				if ((assemblyName.Flags & AssemblyNameFlags.Retargetable) != 0)
				{
					assembly = Assembly.Load(assemblyName);
				}
			}
			return assembly;
		}

		static AssemblyLoader()
		{
			assemblyNames.Add("costura", "costura.costura.dll.compressed");
			symbolNames.Add("costura", "costura.costura.pdb.compressed");
			assemblyNames.Add("microsoft.win32.primitives", "costura.microsoft.win32.primitives.dll.compressed");
			assemblyNames.Add("modernwpf.controls", "costura.modernwpf.controls.dll.compressed");
			assemblyNames.Add("modernwpf", "costura.modernwpf.dll.compressed");
			assemblyNames.Add("netstandard", "costura.netstandard.dll.compressed");
			assemblyNames.Add("system.appcontext", "costura.system.appcontext.dll.compressed");
			assemblyNames.Add("system.collections.concurrent", "costura.system.collections.concurrent.dll.compressed");
			assemblyNames.Add("system.collections", "costura.system.collections.dll.compressed");
			assemblyNames.Add("system.collections.nongeneric", "costura.system.collections.nongeneric.dll.compressed");
			assemblyNames.Add("system.collections.specialized", "costura.system.collections.specialized.dll.compressed");
			assemblyNames.Add("system.componentmodel", "costura.system.componentmodel.dll.compressed");
			assemblyNames.Add("system.componentmodel.eventbasedasync", "costura.system.componentmodel.eventbasedasync.dll.compressed");
			assemblyNames.Add("system.componentmodel.primitives", "costura.system.componentmodel.primitives.dll.compressed");
			assemblyNames.Add("system.componentmodel.typeconverter", "costura.system.componentmodel.typeconverter.dll.compressed");
			assemblyNames.Add("system.console", "costura.system.console.dll.compressed");
			assemblyNames.Add("system.data.common", "costura.system.data.common.dll.compressed");
			assemblyNames.Add("system.diagnostics.contracts", "costura.system.diagnostics.contracts.dll.compressed");
			assemblyNames.Add("system.diagnostics.debug", "costura.system.diagnostics.debug.dll.compressed");
			assemblyNames.Add("system.diagnostics.diagnosticsource", "costura.system.diagnostics.diagnosticsource.dll.compressed");
			assemblyNames.Add("system.diagnostics.fileversioninfo", "costura.system.diagnostics.fileversioninfo.dll.compressed");
			assemblyNames.Add("system.diagnostics.process", "costura.system.diagnostics.process.dll.compressed");
			assemblyNames.Add("system.diagnostics.stacktrace", "costura.system.diagnostics.stacktrace.dll.compressed");
			assemblyNames.Add("system.diagnostics.textwritertracelistener", "costura.system.diagnostics.textwritertracelistener.dll.compressed");
			assemblyNames.Add("system.diagnostics.tools", "costura.system.diagnostics.tools.dll.compressed");
			assemblyNames.Add("system.diagnostics.tracesource", "costura.system.diagnostics.tracesource.dll.compressed");
			assemblyNames.Add("system.diagnostics.tracing", "costura.system.diagnostics.tracing.dll.compressed");
			assemblyNames.Add("system.drawing.primitives", "costura.system.drawing.primitives.dll.compressed");
			assemblyNames.Add("system.dynamic.runtime", "costura.system.dynamic.runtime.dll.compressed");
			assemblyNames.Add("system.globalization.calendars", "costura.system.globalization.calendars.dll.compressed");
			assemblyNames.Add("system.globalization", "costura.system.globalization.dll.compressed");
			assemblyNames.Add("system.globalization.extensions", "costura.system.globalization.extensions.dll.compressed");
			assemblyNames.Add("system.io.compression", "costura.system.io.compression.dll.compressed");
			assemblyNames.Add("system.io.compression.zipfile", "costura.system.io.compression.zipfile.dll.compressed");
			assemblyNames.Add("system.io", "costura.system.io.dll.compressed");
			assemblyNames.Add("system.io.filesystem", "costura.system.io.filesystem.dll.compressed");
			assemblyNames.Add("system.io.filesystem.driveinfo", "costura.system.io.filesystem.driveinfo.dll.compressed");
			assemblyNames.Add("system.io.filesystem.primitives", "costura.system.io.filesystem.primitives.dll.compressed");
			assemblyNames.Add("system.io.filesystem.watcher", "costura.system.io.filesystem.watcher.dll.compressed");
			assemblyNames.Add("system.io.isolatedstorage", "costura.system.io.isolatedstorage.dll.compressed");
			assemblyNames.Add("system.io.memorymappedfiles", "costura.system.io.memorymappedfiles.dll.compressed");
			assemblyNames.Add("system.io.pipes", "costura.system.io.pipes.dll.compressed");
			assemblyNames.Add("system.io.unmanagedmemorystream", "costura.system.io.unmanagedmemorystream.dll.compressed");
			assemblyNames.Add("system.linq", "costura.system.linq.dll.compressed");
			assemblyNames.Add("system.linq.expressions", "costura.system.linq.expressions.dll.compressed");
			assemblyNames.Add("system.linq.parallel", "costura.system.linq.parallel.dll.compressed");
			assemblyNames.Add("system.linq.queryable", "costura.system.linq.queryable.dll.compressed");
			assemblyNames.Add("system.net.http", "costura.system.net.http.dll.compressed");
			assemblyNames.Add("system.net.nameresolution", "costura.system.net.nameresolution.dll.compressed");
			assemblyNames.Add("system.net.networkinformation", "costura.system.net.networkinformation.dll.compressed");
			assemblyNames.Add("system.net.ping", "costura.system.net.ping.dll.compressed");
			assemblyNames.Add("system.net.primitives", "costura.system.net.primitives.dll.compressed");
			assemblyNames.Add("system.net.requests", "costura.system.net.requests.dll.compressed");
			assemblyNames.Add("system.net.security", "costura.system.net.security.dll.compressed");
			assemblyNames.Add("system.net.sockets", "costura.system.net.sockets.dll.compressed");
			assemblyNames.Add("system.net.webheadercollection", "costura.system.net.webheadercollection.dll.compressed");
			assemblyNames.Add("system.net.websockets.client", "costura.system.net.websockets.client.dll.compressed");
			assemblyNames.Add("system.net.websockets", "costura.system.net.websockets.dll.compressed");
			assemblyNames.Add("system.objectmodel", "costura.system.objectmodel.dll.compressed");
			assemblyNames.Add("system.reflection", "costura.system.reflection.dll.compressed");
			assemblyNames.Add("system.reflection.extensions", "costura.system.reflection.extensions.dll.compressed");
			assemblyNames.Add("system.reflection.primitives", "costura.system.reflection.primitives.dll.compressed");
			assemblyNames.Add("system.resources.reader", "costura.system.resources.reader.dll.compressed");
			assemblyNames.Add("system.resources.resourcemanager", "costura.system.resources.resourcemanager.dll.compressed");
			assemblyNames.Add("system.resources.writer", "costura.system.resources.writer.dll.compressed");
			assemblyNames.Add("system.runtime.compilerservices.visualc", "costura.system.runtime.compilerservices.visualc.dll.compressed");
			assemblyNames.Add("system.runtime", "costura.system.runtime.dll.compressed");
			assemblyNames.Add("system.runtime.extensions", "costura.system.runtime.extensions.dll.compressed");
			assemblyNames.Add("system.runtime.handles", "costura.system.runtime.handles.dll.compressed");
			assemblyNames.Add("system.runtime.interopservices", "costura.system.runtime.interopservices.dll.compressed");
			assemblyNames.Add("system.runtime.interopservices.runtimeinformation", "costura.system.runtime.interopservices.runtimeinformation.dll.compressed");
			assemblyNames.Add("system.runtime.numerics", "costura.system.runtime.numerics.dll.compressed");
			assemblyNames.Add("system.runtime.serialization.formatters", "costura.system.runtime.serialization.formatters.dll.compressed");
			assemblyNames.Add("system.runtime.serialization.json", "costura.system.runtime.serialization.json.dll.compressed");
			assemblyNames.Add("system.runtime.serialization.primitives", "costura.system.runtime.serialization.primitives.dll.compressed");
			assemblyNames.Add("system.runtime.serialization.xml", "costura.system.runtime.serialization.xml.dll.compressed");
			assemblyNames.Add("system.security.claims", "costura.system.security.claims.dll.compressed");
			assemblyNames.Add("system.security.cryptography.algorithms", "costura.system.security.cryptography.algorithms.dll.compressed");
			assemblyNames.Add("system.security.cryptography.csp", "costura.system.security.cryptography.csp.dll.compressed");
			assemblyNames.Add("system.security.cryptography.encoding", "costura.system.security.cryptography.encoding.dll.compressed");
			assemblyNames.Add("system.security.cryptography.primitives", "costura.system.security.cryptography.primitives.dll.compressed");
			assemblyNames.Add("system.security.cryptography.x509certificates", "costura.system.security.cryptography.x509certificates.dll.compressed");
			assemblyNames.Add("system.security.principal", "costura.system.security.principal.dll.compressed");
			assemblyNames.Add("system.security.securestring", "costura.system.security.securestring.dll.compressed");
			assemblyNames.Add("system.text.encoding", "costura.system.text.encoding.dll.compressed");
			assemblyNames.Add("system.text.encoding.extensions", "costura.system.text.encoding.extensions.dll.compressed");
			assemblyNames.Add("system.text.regularexpressions", "costura.system.text.regularexpressions.dll.compressed");
			assemblyNames.Add("system.threading", "costura.system.threading.dll.compressed");
			assemblyNames.Add("system.threading.overlapped", "costura.system.threading.overlapped.dll.compressed");
			assemblyNames.Add("system.threading.tasks", "costura.system.threading.tasks.dll.compressed");
			assemblyNames.Add("system.threading.tasks.parallel", "costura.system.threading.tasks.parallel.dll.compressed");
			assemblyNames.Add("system.threading.thread", "costura.system.threading.thread.dll.compressed");
			assemblyNames.Add("system.threading.threadpool", "costura.system.threading.threadpool.dll.compressed");
			assemblyNames.Add("system.threading.timer", "costura.system.threading.timer.dll.compressed");
			assemblyNames.Add("system.valuetuple", "costura.system.valuetuple.dll.compressed");
			assemblyNames.Add("system.xml.readerwriter", "costura.system.xml.readerwriter.dll.compressed");
			assemblyNames.Add("system.xml.xdocument", "costura.system.xml.xdocument.dll.compressed");
			assemblyNames.Add("system.xml.xmldocument", "costura.system.xml.xmldocument.dll.compressed");
			assemblyNames.Add("system.xml.xmlserializer", "costura.system.xml.xmlserializer.dll.compressed");
			assemblyNames.Add("system.xml.xpath", "costura.system.xml.xpath.dll.compressed");
			assemblyNames.Add("system.xml.xpath.xdocument", "costura.system.xml.xpath.xdocument.dll.compressed");
		}

		public static void Attach()
		{
			if (Interlocked.Exchange(ref isAttached, 1) == 1)
			{
				return;
			}
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
			{
				lock (nullCacheLock)
				{
					if (nullCache.ContainsKey(e.Name))
					{
						return null;
					}
				}
				AssemblyName assemblyName = new AssemblyName(e.Name);
				Assembly assembly = ReadExistingAssembly(assemblyName);
				if ((object)assembly != null)
				{
					return assembly;
				}
				assembly = ReadFromEmbeddedResources(assemblyNames, symbolNames, assemblyName);
				if ((object)assembly == null)
				{
					lock (nullCacheLock)
					{
						nullCache[e.Name] = true;
					}
					if ((assemblyName.Flags & AssemblyNameFlags.Retargetable) != 0)
					{
						assembly = Assembly.Load(assemblyName);
					}
				}
				return assembly;
			};
		}
	}
}
