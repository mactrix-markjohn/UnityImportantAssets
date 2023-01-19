using SimpleJSON;

namespace JSONReader.Operations
{
    public class OperationRemove : IJSONNodeOperation
    {
        private readonly JSONArray _array;
        private readonly int _index;

        public OperationRemove(JSONArray array, int index)
        {
            _array = array;
            _index = index;
        }

        public void Execute()
        {
            _array.Remove(_index);
        }
    }
}