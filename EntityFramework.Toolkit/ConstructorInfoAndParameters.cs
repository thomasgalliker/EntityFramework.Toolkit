using System;
using System.Diagnostics;
using System.Reflection;

namespace EntityFramework.Toolkit
{
    [DebuggerDisplay("{this.ConstructorInfo.Name}")]
    public class ConstructorInfoAndParameters
    {
        public ConstructorInfoAndParameters(ConstructorInfo constructorInfo, object[] parameters)
        {
            if (constructorInfo == null)
            {
                throw new ArgumentNullException(nameof(constructorInfo));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            this.ConstructorInfo = constructorInfo;
            this.Parameters = parameters;
        }

        public ConstructorInfo ConstructorInfo { get; private set; }

        public object[] Parameters { get; private set; }

        public object Invoke()
        {
            try
            {
                return this.ConstructorInfo.Invoke(this.Parameters);
            }
            catch (TypeInitializationException tiex)
            {
                if (tiex.InnerException != null)
                {
                    throw tiex.InnerException;
                }

                throw;
            }
        }
    }
}