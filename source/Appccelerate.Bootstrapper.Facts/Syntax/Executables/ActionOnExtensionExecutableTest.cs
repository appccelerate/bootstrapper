//-------------------------------------------------------------------------------
// <copyright file="ActionOnExtensionExecutableTest.cs" company="Appccelerate">
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

namespace Appccelerate.Bootstrapper.Syntax.Executables
{
    using System.Collections.Generic;
    using Appccelerate.Bootstrapper.Dummies;
    using Appccelerate.Bootstrapper.Reporting;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ActionOnExtensionExecutableTest
    {
        private readonly IExecutableContext executableContext;

        private readonly ActionOnExtensionExecutable<ICustomExtension> testee;

        public ActionOnExtensionExecutableTest()
        {
            this.executableContext = A.Fake<IExecutableContext>();

            this.testee = new ActionOnExtensionExecutable<ICustomExtension>(x => x.Dispose());
        }

        [Fact]
        public void Execute_ShouldExecuteActionOnExtensions()
        {
            var firstExtension = A.Fake<ICustomExtension>();
            var secondExtension = A.Fake<ICustomExtension>();

            this.testee.Execute(new List<ICustomExtension> { firstExtension, secondExtension }, this.executableContext);

            A.CallTo(() => firstExtension.Dispose()).MustHaveHappened();
            A.CallTo(() => secondExtension.Dispose()).MustHaveHappened();
        }

        [Fact]
        public void ShoulDescribeItself()
        {
            this.testee.Describe().Should().Be("Executes \"x => x.Dispose()\" on each extension during bootstrapping.");
        }
    }
}