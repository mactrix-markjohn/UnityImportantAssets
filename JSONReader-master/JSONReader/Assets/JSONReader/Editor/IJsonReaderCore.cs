using UnityEngine;

namespace JSONReader
{
    public interface IJsonReaderCore
    {
        void ParseTextFile(string path);
        void ParseTextAsset(TextAsset textAsset);
        void Save();
        void SaveAs(string path);
        void ImportToProject();

        string SelectedFilePath { get; }
    }
}