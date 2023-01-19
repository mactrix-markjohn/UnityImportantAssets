using JSONReader.Operations;
using SimpleJSON;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JSONReader
{
    public class JsonStructureUI
    {
        private GUIStyleProvider _guiStyleProvider;
        private float _keysWidth = 0;
        private Queue<IJSONNodeOperation> _pendingOperations;

        private Dictionary<JSONNode, bool> _foldouts = new Dictionary<JSONNode, bool>();

        private Vector2 _scrollPos;

        public JsonStructureUI(float keysWidth, Queue<IJSONNodeOperation> pendingOperations)
        {
            _keysWidth = keysWidth;
            _pendingOperations = pendingOperations;
            _guiStyleProvider = new GUIStyleProvider(Color.white * GuiConstants.GUI_COLOR_GRAY_PERCENTAGE);
        }

        public void DrawJSONStructure(JSONNode rootNode)
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            EditorGUI.indentLevel++;
            foreach (var node in rootNode)
            {
                DrawJsonNode(node, rootNode);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndScrollView();
        }

        public void DrawJsonNode(KeyValuePair<string, JSONNode> nodeNameValue, JSONNode parent)
        {
            if (nodeNameValue.Value.IsArray)
            {
                GUILayout.BeginVertical(_guiStyleProvider.CurrentStyle);
                DrawArray(nodeNameValue);
                GUILayout.EndVertical();
            }
            else
            {
                if (nodeNameValue.Value.IsObject)
                {
                    GUILayout.BeginVertical(_guiStyleProvider.CurrentStyle);
                    DrawObject(nodeNameValue);
                    GUILayout.EndVertical();
                }
                else
                {
                    GUILayout.BeginHorizontal(_guiStyleProvider.CurrentStyle);
                    DrawValueNode(nodeNameValue, parent);
                    GUILayout.EndHorizontal();
                }
            }
        }

        public void DrawValueNode(KeyValuePair<string, JSONNode> nodeNameValue, JSONNode parentNode)
        {
            GUILayout.Space(EditorGUI.indentLevel * GuiConstants.INDENTATION_SIZE);
            GUILayout.Label(nodeNameValue.Key, GUILayout.Width(_keysWidth));
            if (nodeNameValue.Value.IsNumber)
            {
                nodeNameValue.Value.Value = EditorGUILayout.FloatField(nodeNameValue.Value.AsFloat).ToString();
            }
            else
            {
                if (nodeNameValue.Value.IsBoolean)
                {
                    nodeNameValue.Value.Value = EditorGUILayout.Toggle(nodeNameValue.Value.AsBool).ToString();
                }
                else
                {
                    if (nodeNameValue.Value.IsNull)
                    {
                        GUILayout.Label(GuiConstants.NULL);

                        if (GUILayout.Button(GuiConstants.BUTTON_AS_NUMBER))
                        {
                            _pendingOperations.Enqueue(new OperationReplace(nodeNameValue.Key, parentNode, new JSONNumber(0), nodeNameValue.Value));
                        }
                        if (GUILayout.Button(GuiConstants.BUTTON_AS_STRING))
                        {
                            _pendingOperations.Enqueue(new OperationReplace(nodeNameValue.Key, parentNode, new JSONString(string.Empty), nodeNameValue.Value));
                        }
                        if (GUILayout.Button(GuiConstants.BUTTON_AS_BOOL))
                        {
                            _pendingOperations.Enqueue(new OperationReplace(nodeNameValue.Key, parentNode, new JSONBool(false), nodeNameValue.Value));
                        }
                        if (GUILayout.Button(GuiConstants.BUTTON_AS_ARRAY))
                        {
                            _pendingOperations.Enqueue(new OperationReplace(nodeNameValue.Key, parentNode, new JSONArray(), nodeNameValue.Value));
                        }

                    }
                    else
                    {
                        nodeNameValue.Value.Value = GUILayout.TextField(nodeNameValue.Value.Value);
                    }
                }
            }
        }

        public void DrawArray(KeyValuePair<string, JSONNode> nodeNameValue)
        {
            var array = nodeNameValue.Value.AsArray;
            if (FoldedDraw(string.Format(GuiConstants.ARRAY, nodeNameValue.Key, array.Count), nodeNameValue.Value))
            {
                EditorGUI.indentLevel++;
                foreach (var node in nodeNameValue.Value)
                {
                    _guiStyleProvider.SwitchEvenOdd();
                    GUILayout.BeginHorizontal(_guiStyleProvider.CurrentStyle);
                    GUILayout.Label(array.IndexOf(node).ToString(), GUILayout.Width(GuiConstants.SMALL_BUTTON_WIDTH));
                    DrawJsonNode(node, nodeNameValue.Value);


                    if (nodeNameValue.Value.Count > 1)
                    {
                        if (GUILayout.Button(GuiConstants.BUTTON_UP, GUILayout.Width(GuiConstants.SMALL_BUTTON_WIDTH)))
                        {
                            _pendingOperations.Enqueue(new OperationMoveUp(array, node));
                        }

                        if (GUILayout.Button(GuiConstants.BUTTON_DOWN, GUILayout.Width(GuiConstants.SMALL_BUTTON_WIDTH)))
                        {
                            _pendingOperations.Enqueue(new OperationMoveDown(array, node));
                        }
                    }
                    if (GUILayout.Button(GuiConstants.BUTTON_REMOVE, GUILayout.Width(GuiConstants.SMALL_BUTTON_WIDTH)))
                    {
                        _pendingOperations.Enqueue(new OperationRemove(array, array.IndexOf(node)));
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(_keysWidth + EditorGUI.indentLevel * GuiConstants.INDENTATION_SIZE);

                if (GUILayout.Button(GuiConstants.BUTTON_ADD))
                {
                    _pendingOperations.Enqueue(new OperationAdd(array));
                }

                GUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }
        }

        public void DrawObject(KeyValuePair<string, JSONNode> nodeNameValue)
        {
            if (FoldedDraw(string.Format(GuiConstants.OBJECT, nodeNameValue.Key), nodeNameValue.Value))
            {
                EditorGUI.indentLevel++;
                GUILayout.BeginVertical(_guiStyleProvider.CurrentStyle);
                foreach (var node in nodeNameValue.Value)
                {
                    _guiStyleProvider.SwitchEvenOdd();
                    DrawJsonNode(node, nodeNameValue.Value);
                }
                GUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }
        }

        public bool FoldedDraw(string label, JSONNode node)
        {
            if (!_foldouts.ContainsKey(node))
            {
                _foldouts.Add(node, GuiConstants.INITIAL_FOLDOUT_STATE);
            }

            EditorGUI.indentLevel--;
            bool folded = EditorGUILayout.Foldout(_foldouts[node], label);
            EditorGUI.indentLevel++;

            _foldouts[node] = folded;
            return folded;
        }
    }
}