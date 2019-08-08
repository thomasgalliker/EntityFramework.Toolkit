﻿using System;
using EntityFramework.Toolkit.Concurrency;

namespace ToolkitSample.DataAccess.Concurrency
{
    /// <summary>
    /// Sample implementation for a merging resolve strategy
    /// </summary>
    public class CustomConcurrencyResolveStrategy : IConcurrencyResolveStrategy
    {
        public object ResolveConcurrencyException(object conflictingEntity, object databaseEntity)
        {
            throw new NotImplementedException();
        }
    }
}
