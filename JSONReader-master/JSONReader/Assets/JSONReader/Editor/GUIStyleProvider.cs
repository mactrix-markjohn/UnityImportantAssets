using UnityEngine;

namespace JSONReader
{
    public class GUIStyleProvider
    {
        private GUIStyle _guiStyleEven;
        private GUIStyle _guiStyleOdd;

        private Texture2D _evenTexture;
        private Texture2D _oddTexture;
        public GUIStyle CurrentStyle => _isEven ? _guiStyleEven : _guiStyleOdd;

        private bool _isEven;

        public GUIStyleProvider(Color color)
        {
            Color evenColor = color;
            Color oddColor = color * 0.03f;

            _evenTexture = new Texture2D(1, 1);
            _evenTexture.SetPixel(0, 0, evenColor);
            _evenTexture.Apply();
            _guiStyleEven = new GUIStyle();
            _guiStyleEven.normal.background = _evenTexture;

            _oddTexture = new Texture2D(1, 1);
            _oddTexture.SetPixel(0, 0, oddColor);
            _oddTexture.Apply();
            _guiStyleOdd = new GUIStyle();
            _guiStyleOdd.normal.background = _oddTexture;

            _isEven = true;
        }

        public void SwitchEvenOdd()
        {
            _isEven = !_isEven;
        }
    }
}
