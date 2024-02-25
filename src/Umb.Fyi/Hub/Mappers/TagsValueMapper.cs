using Umbraco.UIBuilder.Mapping;

namespace Umb.Fyi.Hub.Mappers
{
    internal class TagsValueMapper : ValueMapper
    {
        public override object EditorToModel(object input)
        {
            var str = input?.ToString();

            return !string.IsNullOrWhiteSpace(str) ? str.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();
        }

        public override object ModelToEditor(object input)
        {
            var arr = (string[])input;

            return arr != null && arr.Length > 0 ? string.Join(",", arr) : null;
        }
    }
}
