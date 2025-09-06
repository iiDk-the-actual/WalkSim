using System;
using System.Reflection;
using UnityEngine;

namespace WalkSim.WalkSim.Tools
{
    public static class AssetUtils
    {
        private static string FormatPath(string path) => path.Replace("/", ".").Replace("\\", ".");

        public static AssetBundle LoadAssetBundle(string path)
        {
            path = FormatPath(path);
            var manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            var assetBundle = AssetBundle.LoadFromStream(manifestResourceStream);
            manifestResourceStream?.Close();
            return assetBundle;
        }

        public static string[] GetResourceNames() =>
            Assembly.GetCallingAssembly().GetManifestResourceNames();
    }
}