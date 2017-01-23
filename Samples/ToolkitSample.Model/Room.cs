using System.Diagnostics;

namespace ToolkitSample.Model
{
    [DebuggerDisplay("Room: Id={Id}, Level={Level}, Sector={Sector}")]
    public class Room
    {
        public int Id { get; set; }

        public int Level { get; set; }

        public string Sector { get; set; }

        public string Description { get; set; }
    }
}