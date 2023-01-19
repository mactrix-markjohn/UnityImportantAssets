using SimpleJSON;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace JSONReader
{
    public class JsonReaderCore : IJsonReaderCore
    {
        public event Action OnParsingError;

        private string _selectedFilePath;
        private string _selectedFileContent;
        private JSONNode _rootNode;

        public JSONNode RootNode => _rootNode;

        public string SelectedFilePath => _selectedFilePath;

        public void ParseTextFile(string path)
        {
            _selectedFilePath = path;
            _selectedFileContent = File.ReadAllText(SelectedFilePath);
            ParseString(_selectedFileContent);
        }

        public void ParseTextAsset(TextAsset textAsset)
        {
            _selectedFilePath = AssetDatabase.GetAssetPath(textAsset);
            _selectedFileContent = textAsset.text;
            ParseString(_selectedFileContent);
        }

        public bool ParseString(string content)
        {
            try
            {
                _rootNode = JSON.Parse(content);
            }
            catch
            {
                OnParsingError?.Invoke();
                return false;
            }
            return true;
        }

        public void ImportToProject()
        {
            string newPath = Path.Combine(Application.dataPath, Path.GetFileName(_selectedFilePath));
            if (newPath.Length != 0)
            {
                FileUtil.CopyFileOrDirectory(_selectedFilePath, newPath);
                _selectedFilePath = newPath;
                ParseTextFile(_selectedFilePath);
            }
        }

        public void Save()
        {
            if (SelectedFilePath.Length != 0)
            {
                _selectedFileContent = _rootNode.ToString();
                File.WriteAllText(SelectedFilePath, _selectedFileContent);
            }
        }

        public void SaveAs(string path)
        {
            if (path.Length != 0)
            {
                _selectedFilePath = path;
                Save();
            }
        }
    }
}