using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Raven35.Abstractions.Replication;
using Raven35.Client.Linq;

namespace Raven35.Client
{
    public abstract class QueryConvention : ConventionBase
    {
        /// <summary>
        /// Gets or sets the identity parts separator used by the HiLo generators
        /// </summary>
        /// <value>The identity parts separator.</value>
        public string IdentityPartsSeparator { get; set; }

        /// <summary>
        /// Saves Enums as integers and instruct the Linq provider to query enums as integer values.
        /// </summary>
        public bool SaveEnumsAsIntegers { get; set; }

        public delegate LinqPathProvider.Result CustomQueryTranslator(LinqPathProvider provider, Expression expression);

        private readonly Dictionary<MemberInfo, CustomQueryTranslator> customQueryTranslators = new Dictionary<MemberInfo, CustomQueryTranslator>();

        public void RegisterCustomQueryTranslator<T>(Expression<Func<T, object>> member, CustomQueryTranslator translator)
        {
            var body = member.Body as UnaryExpression;
            if (body == null)
                throw new NotSupportedException("A custom query translator can only be used to evaluate a simple member access or method call.");

            var info = GetMemberInfoFromExpression(body.Operand);

            if (!customQueryTranslators.ContainsKey(info))
                customQueryTranslators.Add(info, translator);
        }

        internal LinqPathProvider.Result TranslateCustomQueryExpression(LinqPathProvider provider, Expression expression)
        {
            var member = GetMemberInfoFromExpression(expression);

            CustomQueryTranslator translator;
            if (!customQueryTranslators.TryGetValue(member, out translator))
                return null;

            return translator.Invoke(provider, expression);
        }

        private static MemberInfo GetMemberInfoFromExpression(Expression expression)
        {
            var callExpression = expression as MethodCallExpression;
            if (callExpression != null)
                return callExpression.Method;

            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
                return memberExpression.Member;

            throw new NotSupportedException("A custom query translator can only be used to evaluate a simple member access or method call.");
        }

        internal void UpdateFrom(ReplicationClientConfiguration configuration)
        {
            if (configuration == null)
                return;

            if(ShouldReplaceFailoverBehavior(configuration))
                // ReSharper disable once PossibleInvalidOperationException
                FailoverBehavior = configuration.FailoverBehavior.Value;

            if (configuration.RequestTimeSlaThresholdInMilliseconds.HasValue)
                RequestTimeSlaThresholdInMilliseconds = configuration.RequestTimeSlaThresholdInMilliseconds.Value;
        }

        private bool ShouldReplaceFailoverBehavior(ReplicationClientConfiguration configuration)
        {
            var configurationFailoverBehavior = configuration.FailoverBehavior;
            if (configurationFailoverBehavior.HasValue == false)
                return false;
            if (configuration.OnlyModifyFailoverIfNotInClusterAlready && IsClusterBehavior(FailoverBehavior))
                return false;
            return true;
        }

        private bool IsClusterBehavior(FailoverBehavior configurationFailoverBehavior)
        {
            switch (configurationFailoverBehavior)
            {
                case FailoverBehavior.ReadFromLeaderWriteToLeader:
                case FailoverBehavior.ReadFromLeaderWriteToLeaderWithFailovers:
                case FailoverBehavior.ReadFromAllWriteToLeader:
                case FailoverBehavior.ReadFromAllWriteToLeaderWithFailovers:
                    return true;
                default:
                    return false;
            }
        }
    }
}
