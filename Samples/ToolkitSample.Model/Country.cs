using System.Diagnostics;

namespace ToolkitSample.Model
{
    [DebuggerDisplay("Country: Id={Id}, Name={Name}")]
    public class Country
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}