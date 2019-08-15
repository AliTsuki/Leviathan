using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using UnityEngine;

// Reads or writes to files
public static class FileOps
{
    // Settings file path
    private const string SettingsFilePath = "Settings.json";

    // Read settings from file
    public static bool ReadSettingsFromFile()
    {
        // If settings file does not exist, return false
        if(File.Exists(SettingsFilePath) == false)
        {
            Debug.Log($@"No settings file currently exists.");
            Logger.Log($@"No settings file currently exists.");
            return false;
        }
        // If settings file exists, try reading it
        else
        {
            // Try reading file, if succesful return true, else return false
            try
            {
                PlayerInput.UpdateSettings(JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFilePath)));
                // Check if settings were properly deserialized and file wasn't corrupted
                foreach(KeyValuePair<InputBinding.GameInputsEnum, InputBinding> binding in PlayerInput.InputSettings.InputBindingsKBM)
                {
                    if(PlayerInput.InputBindingsKBMDefault.ContainsKey(binding.Key) == false)
                    {
                        throw new Exception($@"Input Key missing from file, settings file configured improperly or corrupted.");
                    }
                }
                foreach(KeyValuePair<InputBinding.GameInputsEnum, InputBinding> binding in PlayerInput.InputSettings.InputBindingsController)
                {
                    if(PlayerInput.InputBindingsControllerDefault.ContainsKey(binding.Key) == false)
                    {
                        throw new Exception($@"Input Key missing from file, settings file configured improperly or corrupted.");
                    }
                }
                Debug.Log($@"Settings successfully read from file.");
                Logger.Log($@"Settings successfully read from file.");
                return true;
            }
            catch(Exception e)
            {
                Debug.Log($@"Error in reading settings file: " + e.ToString());
                Logger.Log($@"Error in reading settings file: " + e.ToString());
                return false;
            }
        }
    }

    // Write settings to file
    public static void WriteSettingsToFile()
    {
        File.WriteAllText(SettingsFilePath, JsonConvert.SerializeObject(PlayerInput.InputSettings, Formatting.Indented));
        Debug.Log($@"Wrote settings to file.");
        Logger.Log($@"Wrote settings to file.");
    }
}
