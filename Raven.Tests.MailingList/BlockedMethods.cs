// -----------------------------------------------------------------------
//  <copyright file="CanUsePropertyNow.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Linq;
using System.Security;
using Raven35.Abstractions.Exceptions;
using Raven35.Abstractions.Indexing;
using Raven35.Abstractions.MEF;
using Raven35.Database.Config;
using Raven35.Database.Linq;
using Raven35.Database.Plugins;
using Xunit;

namespace Raven35.Tests.MailingList
{
    public class BlockedMethods : IDisposable
    {
        public BlockedMethods()
        {
            CodeVerifier.Active = true;
        }

        [Fact]
        public void CanUseNowProp()
        {
            Compile("from doc in docs select new { doc.Now }");
        }

        [Fact]
        public void CannotDefineLambdaInLet()
        {
            Assert.Throws<SecurityException>(
                () =>
                Compile(
                    "from doc in docs let _ = (Func<int>)(()=>1) select new { doc.Now , die = _()}"));
        }


        [Fact]
        public void CannotDefineLambdaWithExpression()
        {
            Assert.Throws<SecurityException>(
                () =>
                Compile(
                    "from doc in docs select new { doc.Now , die = ((Func<int>)(()=>{ return 1;}))()}"));
        }

        [Fact]
        public void CannotUseEnvironment()
        {
            Assert.Throws<SecurityException>(() => Compile(
                "from doc in docs  select new { doc.Now , die = ((Func<int>)(()=>{ System.Environment.Exit(1); return 1; }))()}"));
        }

        [Fact]
        public void CannotUseTaskFactory()
        {
            Assert.Throws<SecurityException>(() => Compile(
                "from doc in docs  select new { doc.Now , die = System.Threading.Tasks.Task.Factory.StartNew((Func<int>)(()=>{ return 1; }))}"));
        }

        [Fact]
        public void CannotUseTaskStart()
        {
            Assert.Throws<SecurityException>(() => Compile(
                "from doc in docs  select new { doc.Now , die = ((Func<int>)(()=>{ new System.Threading.Tasks.Task(()=>{}).Start(); return 1; }))()}"));
        }

        [Fact]
        public void CannotUseIO()
        {
            Assert.Throws<SecurityException>(() => Compile(
                "from doc in docs  select new { doc.Now , die = ((Func<int>)(()=>{ System.IO.File.Delete(\"test\"); return 1; }))()}"));
        }

        [Fact]
        public void CannotCreateTask()
        {
            Assert.Throws<SecurityException>(() => Compile(
                "from doc in docs  select new { doc.Now , die = ((Func<int>)(()=>{ new System.Threading.Tasks.Task(()=>{}); return 1; }))()}"));
        }

        [Fact]
        public void CannotStartThread()
        {
            Assert.Throws<SecurityException>(() => Compile(
                    "from doc in docs  select new { doc.Now , die = ((Func<int>)(()=>{ new System.Threading.Thread(()=>{}).Start(); return 1; }))()}"));
        }

        [Fact]
        public void CannotUseDatetimeNowProp()
        {
            Assert.Throws<IndexCompilationException>(() => Compile("from doc in docs select new { DateTime.Now }"));
        }

        [Fact]
        public void CannotUseSystemDatetimeNowProp()
        {
            Assert.Throws<IndexCompilationException>(() => Compile("from doc in docs select new { System.DateTime.Now }"));
        }

        private void Compile(string code)
        {
            var configuration = new InMemoryRavenConfiguration();
            configuration.Initialize();
            var dynamicViewCompiler = new DynamicViewCompiler("test", new IndexDefinition
            {
                Map = code
            }, new OrderedPartCollection<AbstractDynamicCompilationExtension>(), ".", configuration);
            dynamicViewCompiler.GenerateInstance();
        }

        public void Dispose()
        {
            CodeVerifier.Active = false;
        }
    }
}
