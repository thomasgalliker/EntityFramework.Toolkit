using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EntityFramework.Toolkit.Core
{
    [DebuggerDisplay("ChangeSet: Context='{this.Context.Name}', Changes={this.Changes.Count()}", Type = "Change")]
    public class ChangeSet
    {
        public static readonly ChangeSet Empty = new ChangeSet(null, new List<IChange>());

        public ChangeSet(Type contextType, IList<IChange> changes)
        {
            this.Context = contextType;
            this.Changes = changes ?? Enumerable.Empty<IChange>();
        }

        public Type Context { get; private set; }

        public IEnumerable<IChange> Changes { get; private set; }
    }
}