using System.Diagnostics;

namespace ToolkitSample.Model
{
    [DebuggerDisplay("Id={Id}, Path={Path}")]
    public class ApplicationSetting
    {
        public int Id { get { return 1; } set { } }

        public string Path { get; set; }
    }
}