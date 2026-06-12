using System.IO;
using System.Diagnostics;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;

public class MacDesktopPetPostprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.StandaloneOSX)
        {
            return;
        }

        BuildNativePlugin();
    }

    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.StandaloneOSX)
        {
            return;
        }

        string plistPath = Path.Combine(pathToBuiltProject, "Contents/Info.plist");
        if (!File.Exists(plistPath))
        {
            return;
        }

        XDocument document = XDocument.Load(plistPath);
        XElement dict = document.Root?.Element("dict");
        if (dict == null)
        {
            return;
        }

        SetBool(dict, "LSUIElement", false);
        SetBool(dict, "NSHighResolutionCapable", true);

        document.Save(plistPath);
    }

    private static void BuildNativePlugin()
    {
        string sourcePath = Path.Combine("Assets", "Plugins", "macOS", "DesktopPetMac.mm");
        string bundleMacOSPath = Path.Combine("Assets", "Plugins", "macOS", "DesktopPetMac.bundle", "Contents", "MacOS");
        string outputPath = Path.Combine(bundleMacOSPath, "DesktopPetMac");

        Directory.CreateDirectory(bundleMacOSPath);

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "xcrun",
            Arguments = "clang++ -arch arm64 -arch x86_64 -x objective-c++ -fobjc-arc -bundle -framework Cocoa -framework QuartzCore "
                + Quote(sourcePath) + " -o " + Quote(outputPath),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using (Process process = Process.Start(startInfo))
        {
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                throw new BuildFailedException("DesktopPetMac.bundle 编译失败。\n" + output + error);
            }
        }
    }

    private static string Quote(string value)
    {
        return "\"" + value.Replace("\"", "\\\"") + "\"";
    }

    private static void SetBool(XElement dict, string key, bool value)
    {
        XElement keyElement = FindKey(dict, key);
        XElement valueElement = value ? new XElement("true") : new XElement("false");

        if (keyElement == null)
        {
            dict.Add(new XElement("key", key));
            dict.Add(valueElement);
            return;
        }

        XElement oldValueElement = FirstOrDefaultElement(keyElement.ElementsAfterSelf());
        if (oldValueElement == null)
        {
            keyElement.AddAfterSelf(valueElement);
            return;
        }

        oldValueElement.ReplaceWith(valueElement);
    }

    private static XElement FindKey(XElement dict, string key)
    {
        foreach (XElement element in dict.Elements("key"))
        {
            if (element.Value == key)
            {
                return element;
            }
        }

        return null;
    }

    private static XElement FirstOrDefaultElement(System.Collections.Generic.IEnumerable<XElement> elements)
    {
        foreach (XElement element in elements)
        {
            return element;
        }

        return null;
    }
}
