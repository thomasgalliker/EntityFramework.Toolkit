using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EntityFramework.Toolkit
{
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