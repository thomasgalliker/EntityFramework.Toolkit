using System.Collections.Generic;
using System.Diagnostics;

namespace ToolkitSample.DataAccess.Model
{
    [DebuggerDisplay("Id={Id}, Name={Name}")]
    public class Department
    {
        public Department()
        {
            this.Employees = new HashSet<Employee>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int? LeaderId { get; set; }

        public virtual Employee Leader { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}