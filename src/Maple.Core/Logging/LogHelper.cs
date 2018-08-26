using Maple.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Logging
{
    public static class LogHelper
    {
        public static ILogger Logger { get; private set; }

        static LogHelper()
        {
            try
            {
                ILoggerFactory loggerFactory = EngineContext.Current.Resolve<ILoggerFactory>();
                if (loggerFactory != null)
                    Logger  = loggerFactory.CreateLogger(typeof(LogHelper));
                else
                    Logger = NullLogger.Instance;
            }
            catch { }
        }

        public static void LogException(Exception ex)
        {
            LogException(Logger, ex);
        }

        public static void LogException(ILogger logger, Exception ex)
        {
            if (ex == null)
                logger.LogError(500, "捕获一个NULL空异常");
            else
            {
                string message = ex.Message;
                string stackTrace = ex.StackTrace;

                logger.LogError(500, "Message:{message}\r\nStackTrace:{stackTrace}", message, stackTrace);
            }

            //var severity = (ex as IHasLogSeverity)?.Severity ?? LogSeverity.Error;

            //logger.Log(LogLevel.Error,500,  ex.Message, ex);

            //LogValidationErrors(logger, ex);
        }

        //private static void LogValidationErrors(ILogger logger, Exception exception)
        //{
        //    //Try to find inner validation exception
        //    if (exception is AggregateException && exception.InnerException != null)
        //    {
        //        var aggException = exception as AggregateException;
        //        if (aggException.InnerException is AbpValidationException)
        //        {
        //            exception = aggException.InnerException;
        //        }
        //    }

        //    if (!(exception is AbpValidationException))
        //    {
        //        return;
        //    }

        //    var validationException = exception as AbpValidationException;
        //    if (validationException.ValidationErrors.IsNullOrEmpty())
        //    {
        //        return;
        //    }

        //    logger.Log(validationException.Severity, "There are " + validationException.ValidationErrors.Count + " validation errors:");
        //    foreach (var validationResult in validationException.ValidationErrors)
        //    {
        //        var memberNames = "";
        //        if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
        //        {
        //            memberNames = " (" + string.Join(", ", validationResult.MemberNames) + ")";
        //        }

        //        logger.Log(validationException.Severity, validationResult.ErrorMessage + memberNames);
        //    }
        //}
    }
}
