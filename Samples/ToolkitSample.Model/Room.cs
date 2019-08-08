﻿using System;
using System.Diagnostics;
using EntityFramework.Toolkit.Core.Auditing;

namespace ToolkitSample.Model
{
    [DebuggerDisplay("Room: Id={Id}, Level={Level}, Sector={Sector}")]
    public class Room : ICreatedDate
    {
        public int Id { get; set; }

        public int Level { get; set; }

        public string Sector { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}