using System;
using Raven35.Abstractions.Logging;
using Raven35.Tests.Common;
using Sparrow35.Collections;
using Xunit;

namespace Raven35.Tests.Abstractions.Logging
{
    public class LoggerExecutionWrapperTests : NoDisposalNeeded
    {
        private readonly LoggerExecutionWrapper sut;
        private readonly FakeLogger fakeLogger;

        public LoggerExecutionWrapperTests()
        {
            fakeLogger = new FakeLogger();
            sut = new LoggerExecutionWrapper(fakeLogger, "name", new ConcurrentSet<Target>());
        }

        [Fact]
        public void When_logging_and_message_factory_throws_Then_should_log_exception()
        {
            var loggingException = new Exception("Message");
            sut.Log(LogLevel.Info, () => { throw loggingException; });
            Assert.Same(loggingException, fakeLogger.Exception);
            Assert.Equal(LoggerExecutionWrapper.FailedToGenerateLogMessage, fakeLogger.Message);
        }

        [Fact]
        public void When_logging_with_exception_and_message_factory_throws_Then_should_log_exception()
        {
            var appException = new Exception("Message");
            var loggingException = new Exception("Message");
            sut.Log(LogLevel.Info, () => { throw loggingException; }, appException);
            Assert.Same(loggingException, fakeLogger.Exception);
            Assert.Equal(LoggerExecutionWrapper.FailedToGenerateLogMessage, fakeLogger.Message);
        }

        public class FakeLogger : ILog
        {
            private LogLevel logLevel;

            public LogLevel LogLevel
            {
                get { return logLevel; }
            }

            public string Message
            {
                get { return message; }
            }

            public Exception Exception
            {
                get { return exception; }
            }

            private string message;
            private Exception exception;

            public bool IsInfoEnabled { get; set; }

            public bool IsDebugEnabled { get; set; }

            public bool IsWarnEnabled { get; set; }

            public void Log(LogLevel logLevel, Func<string> messageFunc)
            {
                messageFunc();
            }

            public void Log<TException>(LogLevel logLevel, Func<string> messageFunc, TException exception) where TException : Exception
            {
                string messageResult = messageFunc();
                if (messageResult != null)
                {
                    this.logLevel = logLevel;
                    this.message = messageResult;
                    this.exception = exception;
                }
            }

            public bool ShouldLog(LogLevel logLevel)
            {
                return true;
            }
        }
    }
}
