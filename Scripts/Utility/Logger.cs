using System.Collections.Generic;

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
        System.IO.File.WriteAllLines("Log.txt", MainLogTxt.ToArray());
    }

    // Logger On Application Quit
    public static void OnApplicationQuit()
    {
        System.IO.File.WriteAllLines("Log.txt", MainLogTxt.ToArray());
    }

    // Logging methods
    public static void Log(string _ll)
    {
        MainLogTxt.Add(_ll);
    }
    public static void Log(System.Exception _e)
    {
        MainLogTxt.Add(_e.ToString());
    }
}