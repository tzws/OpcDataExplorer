using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Reflection;


internal class DllHelper
{
    static DllHelper()
    {
        DllHelper.systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\";
        DllHelper.windowsPath = DllHelper.systemPath.Substring(0, DllHelper.systemPath.LastIndexOf("system32"));
        DllHelper.dllAndExeConfig = new string[][]
		{
			new string[]
			{
				"OpcEnum.exe",
				AppDomain.CurrentDomain.BaseDirectory + "OPCDLLS\\OpcEnum.exe",
				DllHelper.systemPath
			},
			new string[]
			{
				"OPC_AEPS.DLL",
				AppDomain.CurrentDomain.BaseDirectory + "OPCDLLS\\OPC_AEPS.DLL",
				DllHelper.systemPath
			},
			new string[]
			{
				"opccomn_ps.dll",
				AppDomain.CurrentDomain.BaseDirectory + "OPCDLLS\\opccomn_ps.dll",
				DllHelper.systemPath
			},
			new string[]
			{
				"opchda_ps.dll",
				AppDomain.CurrentDomain.BaseDirectory + "OPCDLLS\\opchda_ps.dll",
				DllHelper.systemPath
			},
			new string[]
			{
				"OPCPROXY.DLL",
				AppDomain.CurrentDomain.BaseDirectory + "OPCDLLS\\OPCPROXY.DLL",
				DllHelper.systemPath
			},
			new string[]
			{
				"OpcRcw.Comn.dll",
				AppDomain.CurrentDomain.BaseDirectory + "OPCDLLS\\OpcRcw.Comn.dll",
				DllHelper.systemPath
			},
			new string[]
			{
				"OPCDAAuto.dll",
				AppDomain.CurrentDomain.BaseDirectory + "OPCDLLS\\OPCDAAuto.dll",
				DllHelper.systemPath
			}
		};
    }

    internal DllHelper()
    {
    }

    internal static byte[] loadDll(string dllName)
    {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        Console.WriteLine(executingAssembly.GetName().Name);
        Console.WriteLine(executingAssembly.GetName());
        string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        foreach (string x in resourceNames)
        {
            Console.WriteLine(x);
        }


        string tmp = executingAssembly.GetName().Name + ".OPCDlls." + dllName;

        Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + ".OPCDlls." + dllName);
        //Stream manifestResourceStream1 = executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + "." + dllName);
        //Stream manifestResourceStream2 = executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + "." + dllName);
        //Stream manifestResourceStream3 = executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + "." + dllName);

        byte[] array = new byte[manifestResourceStream.Length];
        if (manifestResourceStream != null)
        {
            manifestResourceStream.Read(array, 0, (int)manifestResourceStream.Length);
            manifestResourceStream.Close();
        }

        return array;
    }

    internal static void WriteToRunPath(byte[] byte_0, string string_0)
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        FileStream fileStream = new FileStream(baseDirectory + string_0, FileMode.OpenOrCreate);
        fileStream.Write(byte_0, 0, byte_0.Length);
        fileStream.Flush();
        fileStream.Close();
    }

    internal static void WriteToGivenPath(byte[] byte_0, string string_0)
    {
        int num = string_0.LastIndexOf('\\');
        string text = string_0.Substring(0, num);
        if (!Directory.Exists(text))
        {
            Directory.CreateDirectory(text);
        }
        FileStream fileStream = new FileStream(string_0, FileMode.OpenOrCreate);
        fileStream.Write(byte_0, 0, byte_0.Length);
        fileStream.Flush();
        fileStream.Close();
    }


    private static void WriteDlls()
    {
        string[][] array = DllHelper.dllAndExeConfig;
        for (int i = 0; i < array.Length; i++)
        {
            string[] array2 = array[i];
            string text = array2[0];
            string text2 = array2[1];
            if (!File.Exists(text2))
            {
                byte[] byte_ = loadDll(text);
                WriteToGivenPath(byte_, text2);
            }
        }
    }

    internal static void RegisterDllAndExe()
    {
        if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Interop.OPCAutomation.dll"))
        {
            byte[] byte_ = loadDll("Interop.OPCAutomation.dll");
            WriteToRunPath(byte_, "Interop.OPCAutomation.dll");
        }
        bool flag = false;
        string[][] array = DllHelper.dllAndExeConfig;
        for (int i = 0; i < array.Length; i++)
        {
            string[] array2 = array[i];
            string fullPath = array2[2] + array2[0];
            if (!File.Exists(fullPath))
            {
                flag = true;
                if (flag)
                {
                    DllHelper.WriteDlls();
                    Process process = new Process();
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = false;
                    process.Start();
                    string[][] array3 = DllHelper.dllAndExeConfig;
                    for (int j = 0; j < array3.Length; j++)
                    {
                        string[] array4 = array3[j];
                        string text2 = "Copy \"" + array4[1] + "\" " + array4[2];
                        string filePath = array4[2] + array4[0];
                        if (File.Exists(filePath))
                        {
                            try
                            {
                                File.Delete(filePath);
                                Thread.Sleep(100);
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                        }
                        //Console.WriteLine("text2 is " + text2);
                        process.StandardInput.WriteLine(text2);
                        Thread.Sleep(500);
                        string text4 = "\"" + array4[2] + array4[0] + "\"";
                        if (array4[0].Equals("OpcEnum.exe"))
                        {
                            text4 += " /RegServer";
                            process.StandardInput.WriteLine(text4);
                        }
                        else
                        {
                            text4 = "REGSVR32  /s " + text4;
                            process.StandardInput.WriteLine(text4);
                        }
                        //Console.WriteLine("text4 is " + text4);
                    }
                    process.StandardInput.WriteLine("exit");
                    process.WaitForExit();
                    process.Close();
                }
                DllHelper.deleteDllDirectory();
                return;
            }
        }
    }

    private static void deleteDllDirectory()
    {
        string text = AppDomain.CurrentDomain.BaseDirectory + "OPCDLLS";
        if (Directory.Exists(text))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(text);
            directoryInfo.Delete(true);
        }
    }

    internal static void UnregisterAndDelete()
    {
        Process process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.Start();
        string[][] array = DllHelper.dllAndExeConfig;
        for (int i = 0; i < array.Length; i++)
        {
            string[] array2 = array[i];
            string fullPath = array2[2] + array2[0];
            if (File.Exists(fullPath))
            {
                if (!array2[0].Equals("OPCDAAuto.dll"))
                {
                    if (array2[0].Equals("OpcEnum.exe"))
                    {
                        string command = fullPath + " /UnRegServer";
                        process.StandardInput.WriteLine(command);
                    }
                    else
                    {
                        string command = "REGSVR32 /s /u " + fullPath;
                        process.StandardInput.WriteLine(command);
                    }
                }
                File.Delete(fullPath);
            }
        }
        process.StandardInput.WriteLine("exit");
        process.WaitForExit();
        process.Close();
    }

    private static string systemPath;

    private static string windowsPath;

    private static string[][] dllAndExeConfig;
}
