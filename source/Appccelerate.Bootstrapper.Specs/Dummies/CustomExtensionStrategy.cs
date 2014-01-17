//-------------------------------------------------------------------------------
// <copyright file="CustomExtensionStrategy.cs" company="Appccelerate">
//   Copyright (c) 2008-2013
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Appccelerate.Bootstrapper.Dummies
{
    using System.Collections.Generic;
    using System.Globalization;
    using Appccelerate.Bootstrapper.Syntax;

    public class CustomExtensionStrategy : AbstractStrategy<ICustomExtension>
    {
        private readonly Queue<string> sequenceQueue;

        public CustomExtensionStrategy(Queue<string> sequenceQueue)
        {
            this.sequenceQueue = sequenceQueue;
        }

        public int RunConfigurationInitializerAccessCounter
        {
            get; private set;
        }

        public int ShutdownConfigurationInitializerAccessCounter
        {
            get;
            private set;
        }

        protected override void DefineRunSyntax(ISyntaxBuilder<ICustomExtension> builder)
        {
            builder
                .Execute(() => this.DumpAction("CustomRun"))
                .Execute(extension => extension.Start())
                .Execute(() => this.RunInitializeConfiguration(), (extension, dictionary) => extension.Configure(dictionary))
                .Execute(extension => extension.Initialize())
                .Execute(() => "RunTest", (extension, ctx) => extension.Register(ctx));
        }

        protected override void DefineShutdownSyntax(ISyntaxBuilder<ICustomExtension> syntax)
        {
            syntax
                .Execute(() => this.DumpAction("CustomShutdown"))
                .Execute(() => "ShutdownTest", (extension, ctx) => extension.Unregister(ctx))
                .Execute(() => this.ShutdownInitializeConfiguration(), (extension, dictionary) => extension.DeConfigure(dictionary))
                .Execute(extension => extension.Stop());
        }

        private IDictionary<string, string> RunInitializeConfiguration()
        {
            this.RunConfigurationInitializerAccessCounter++;

            return new Dictionary<string, string> { { "RunTest", "RunTestValue" } };
        }

        private IDictionary<string, string> ShutdownInitializeConfiguration()
        {
            this.ShutdownConfigurationInitializerAccessCounter++;

            return new Dictionary<string, string> { { "ShutdownTest", "ShutdownTestValue" } };
        }

        private void DumpAction(string action)
        {
            this.sequenceQueue.Enqueue(string.Format(CultureInfo.InvariantCulture, "Action: {0}", action));
        }
    }
}