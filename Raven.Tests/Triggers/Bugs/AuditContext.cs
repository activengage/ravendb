using System;
using Raven35.Abstractions.Extensions;
using Raven35.Database.Data;

namespace Raven35.Tests.Triggers.Bugs
{
    public static class AuditContext
    {
        [ThreadStatic]
        private static bool _currentlyInContext;

        public static bool IsInAuditContext
        {
            get
            {
                return _currentlyInContext;
            }
        }

        public static IDisposable Enter()
        {
            var old = _currentlyInContext;
            _currentlyInContext = true;
            return new DisposableAction(() => _currentlyInContext = old);
        }
    }
}
