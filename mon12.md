```
        // Get the current assembly
        var assembly = Assembly.GetExecutingAssembly();

        // Get the namespace of the current program
        string resourceNamespace = assembly.GetName().Name; // This gets the default namespace

        // Assuming your script is named "profiles.ps1" and in the root of your project
        string resourceName = $"{resourceNamespace}.profiles.ps1";

        // Extract the script content from the embedded resource
        string scriptContent;
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new StreamReader(stream))
        {
            scriptContent = reader.ReadToEnd();
        }

        // Save the script to a temporary file
        string tempScriptPath = Path.Combine(Path.GetTempPath(), "profiles.ps1");
        File.WriteAllText(tempScriptPath, scriptContent);

        // Prepare to run the script using ProcessStartInfo
        string arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{tempScriptPath}\"";


```