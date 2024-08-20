using System;

namespace Raven35.Abstractions.Logging
{
    public interface ILogManager
    {
        ILog GetLogger(string name);

        IDisposable OpenNestedConext(string message);

        IDisposable OpenMappedContext(string key, string value);
    }
}
