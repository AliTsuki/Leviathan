using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

// Class for Logging-to-file functions
public static class Logger
{
    // Log path
    private const string LogDir = @"Logs";
    private const string LogPath = @"Logs\Log-";
    private static string CurrentLogPath = "";

    // Max logs
    private const int MaxLogs = 10;

    // Log
    private static List<string> MainLogTxt = new List<string>();


    // Initialize
    public static void Initialize()
    {
        // If log directory doesn't exist, create it
        if(Directory.Exists(LogDir) == false)
        {
            Directory.CreateDirectory(LogDir);
        }
        // Set current log path to include datetime
        CurrentLogPath = LogPath + DateTime.Now.ToString("MM-dd-yyyy-HH-mm-ss") + ".txt";
        // Get a list of all logs
        string[] logs = Directory.GetFiles(LogDir, "Log-*.txt");
        // Get number of logs
        int numLogs = logs.Length;
        // If number of logs is greater than max allowed
        if(numLogs > MaxLogs)
        {
            // Create an array of datetimes length of number of logs
            DateTime[] logTimes = new DateTime[numLogs];
            // Loop through logs
            for(int i = 0; i < numLogs; i++)
            {
                // Set logTimes to parsed datetime of log name
                logTimes[i] = DateTime.ParseExact(logs[i].Substring(9, 19), "MM-dd-yyyy-HH-mm-ss", CultureInfo.InvariantCulture);
            }
            // Get the earliest date in logTimes
            DateTime earliest = logTimes.Min(date => date);
            // Loop through logTimes
            foreach(DateTime time in logTimes)
            {
                // If current time is the earliest time
                if(time == earliest)
                {
                    // Convert time back to name of oldest log
                    string logName = LogDir + $@"\Log-{time.ToString("MM-dd-yyyy-HH-mm-ss")}.txt";
                    // Check that that log exists in logs
                    if(logs.Contains(logName) == true)
                    {
                        // If there is a file for that log
                        if(File.Exists(LogDir + @"\" + logName))
                        {
                            // Delete oldest log and break out of loop
                            File.Delete(LogDir + @"\" + logName);
                            break;
                        }
                    }
                }
            }
        }
    }

    // Logger Update: Write mainlogtxt to Log.txt FILE
    public static void Update()
    {
        File.WriteAllLines(CurrentLogPath, MainLogTxt.ToArray());
    }

    // Logger On Application Quit
    public static void OnApplicationQuit()
    {
        File.WriteAllLines(CurrentLogPath, MainLogTxt.ToArray());
    }

    // Logging methods
    public static void Log(string text)
    {
        MainLogTxt.Add($@"{DateTime.Now.ToString("[MM/dd/yyyy HH:mm:ss]")} - {text}");
    }
    public static void Log(System.Exception error)
    {
        MainLogTxt.Add($@"{DateTime.Now.ToString("[MM/dd/yyyy HH:mm:ss]")} - {error.ToString()}");
    }
}