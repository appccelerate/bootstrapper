//-------------------------------------------------------------------------------
// <copyright file="ActionExecutableTest.cs" company="Appccelerate">
//   Copyright (c) 2008-2014
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

namespace Appccelerate.Bootstrapper.Syntax.Executables
{
    using System.Collections.Generic;
    using System.Linq;
    using Appccelerate.Bootstrapper.Reporting;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ActionExecutableTest
    {
        private readonly IExecutableContext executableContext;

        private readonly ActionExecutable<IExtension> testee;

        private bool wasExecuted;

        public ActionExecutableTest()
        {
            this.executableContext = A.Fake<IExecutableContext>();

            this.testee = new ActionExecutable<IExtension>(() => this.SetWasExecuted());
        }

        [Fact]
        public void Execute_ShouldExecuteAction()
        {
            this.testee.Execute(Enumerable.Empty<IExtension>(), this.executableContext);

            this.wasExecuted.Should().BeTrue();
        }

        [Fact]
        public void Execute_ShouldNotProcessExtensions()
        {
            var enumerator = A.Fake<IEnumerable<IExtension>>();

            this.testee.Execute(enumerator, this.executableContext);

            A.CallTo(() => enumerator.GetEnumerator()).MustNotHaveHappened();
        }

        [Fact]
        public void ShouldDescribeItself()
        {
            this.testee.Describe().Should().Be("Executes \"() => value(Appccelerate.Bootstrapper.Syntax.Executables.ActionExecutableTest).SetWasExecuted()\" during bootstrapping.");
        }

        private bool SetWasExecuted()
        {
            return this.wasExecuted = true;
        }
    }
}