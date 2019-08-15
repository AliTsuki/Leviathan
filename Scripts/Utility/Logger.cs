using System;
using System.Collections.Generic;
using System.IO;

// Class for Logging-to-file functions
public static class Logger
{
    // Logger fields
    private static List<string> MainLogTxt = new List<string>();

    // Initialize
    public static void Initialize()
    {

    }

    // Logger Update: Write mainlogtxt to Log.txt FILE
    public static void Update()
    {
        File.WriteAllLines("Log.txt", MainLogTxt.ToArray());
    }

    // Logger On Application Quit
    public static void OnApplicationQuit()
    {
        File.WriteAllLines("Log.txt", MainLogTxt.ToArray());
    }

    // Logging methods
    public static void Log(string text)
    {
        MainLogTxt.Add($@"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")} - {text}");
    }
    public static void Log(System.Exception error)
    {
        MainLogTxt.Add($@"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")} - {error.ToString()}");
    }
}