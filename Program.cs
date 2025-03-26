using Reloaded.Memory.Sigscan;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Threading;

public class Program
{
    public static void Main()
    {
        Console.Title = "CheatEngineDetector | Made by https://github.com/ZygoteCode/";

        while (true)
        {
            Console.WriteLine(IsCheatEngineRunning() ? $"[{DateTime.Now.ToLongTimeString()}] Cheat Engine is running." : $"[{DateTime.Now.ToLongTimeString()}] Cheat Engine is not running.");
            Thread.Sleep(1000);
        }
    }

    private static string[] blockedPatterns = new string[]
    {
        "73 79 73 63 6F 6E 73 74 2E 73 69 6E 76 61 6C 69 64 69 6E 70 75 74",
    };

    private static string[] blockedProcessNames = new string[]
    {
        "Cheat Engine",
        "FUDCE"
    };

    private static string[] blockedWindowTitles = new string[]
    {
        "Cheat Engine",
        "FUDCE App CE"
    };

    private static FileList[] blockedFileLists = new FileList[]
    {
        new FileList(new string[] { "ced3d9hook64.dll" }),
        new FileList(new string[] { "ced3d10hook64.dll" }),
        new FileList(new string[] { "ced3d11hook64.dll" }),
    };

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    private static string GetCaptionOfActiveWindow()
    {
        var strTitle = string.Empty;
        var handle = GetForegroundWindow();
        var intLength = GetWindowTextLength(handle) + 1;
        var stringBuilder = new StringBuilder(intLength);

        if (GetWindowText(handle, stringBuilder, intLength) > 0)
        {
            strTitle = stringBuilder.ToString();
        }

        return strTitle;
    }

    public static bool IsCheatEngineRunning()
    {
        try
        {
            string actualWindow = GetCaptionOfActiveWindow();

            if (actualWindow != "CheatEngineDetector | Made by https://github.com/ZygoteCode/")
            {
                string filteredWindow = Utils.FilterString(actualWindow);

                foreach (string windowTitle in blockedWindowTitles)
                {
                    string newWindowTitle = Utils.FilterString(windowTitle);

                    if (filteredWindow.Contains(newWindowTitle))
                    {
                        return true;
                    }
                }
            }

            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    if (process.Id == Process.GetCurrentProcess().Id)
                    {
                        continue;
                    }

                    if (process.MainWindowHandle != null && process.MainWindowHandle != new IntPtr(-1) && process.MainWindowHandle != IntPtr.Zero && process.Id != Process.GetCurrentProcess().Id && !Utils.FilterString(process.MainWindowTitle).Equals(""))
                    {
                        if (process.MainWindowTitle == actualWindow)
                        {
                            string filteredProcessName = Utils.FilterString(process.ProcessName);

                            foreach (string processName in blockedProcessNames)
                            {
                                string newProcessName = Utils.FilterString(processName);

                                if (processName.Contains(newProcessName))
                                {
                                    return true;
                                }
                            }

                            foreach (FileList fileList in blockedFileLists)
                            {
                                int existsAll = 0;

                                foreach (string theFile in fileList.List)
                                {
                                    foreach (string file in System.IO.Directory.GetFiles(Utils.GetPathFromFileName(ModuleFileName.GetExecutablePath(process.Id))))
                                    {
                                        if (Utils.FilterString(theFile).Equals(Utils.FilterString(System.IO.Path.GetFileName(file))))
                                        {
                                            existsAll++;
                                            break;
                                        }
                                    }
                                }

                                if (existsAll == fileList.List.Length)
                                {
                                    return true;
                                }
                            }

                            Scanner scanner = new Scanner(process, process.MainModule);

                            foreach (string pattern in blockedPatterns)
                            {
                                var result = scanner.FindPattern(pattern);

                                if (result.Found)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }
        catch
        {

        }

        return false;
    }
}
