using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EntityFramework.Toolkit
{
    public class ChangeSet : ReadOnlyCollection<IChange>
    {
        private static readonly ChangeSet EmptyChangeSet = new ChangeSet(new List<IChange>());

        public ChangeSet(IList<IChange> changes)
            : base(changes)
        {
        }
    }
}