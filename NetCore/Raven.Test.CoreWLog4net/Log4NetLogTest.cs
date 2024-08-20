using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Raven35.Abstractions.Logging;
using Xunit;

namespace Raven35.Test.CoreWLog4net
{
    public class Log4NetLogTest
    {

        [Fact]
        public void CreateLogger()
        {
            LoggerExecutionWrapper log = (LoggerExecutionWrapper)LogManager.GetLogger(typeof(Log4NetLogTest));
            Assert.NotNull(log);
            var lewType = typeof(LoggerExecutionWrapper);
            var loggerField = lewType.GetField("logger", BindingFlags.NonPublic | BindingFlags.Instance);
            var internalLogger = loggerField.GetValue(log);
            Assert.NotNull(internalLogger);
            Assert.False(internalLogger is LogManager.NoOpLogger);
            Assert.True(internalLogger is Raven35.Abstractions.Logging.LogProviders.Log4NetLogManager.Log4NetLogger);

        }
    }
}
