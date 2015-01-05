//-------------------------------------------------------------------------------
// <copyright file="ActionOnExtensionWithInitializerExecutableTest.cs" company="Appccelerate">
//   Copyright (c) 2008-2015
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

    public class ActionOnExtensionWithInitializerExecutableTest
    {
        private readonly object context;

        private readonly ActionOnExtensionWithInitializerExecutable<object, ICustomExtension> testee;

        private readonly IExecutableContext executableContext;

        private int contextAccessCounter;

        private IBehaviorAware<IExtension> interceptedBehaviorAware;

        public ActionOnExtensionWithInitializerExecutableTest()
        {
            this.executableContext = A.Fake<IExecutableContext>();

            this.context = new object();

            this.testee = new ActionOnExtensionWithInitializerExecutable<object, ICustomExtension>(
                () => this.CountAccessToContext(),
                (x, i) => x.SomeMethod(i),
                (aware, ctx) => { this.interceptedBehaviorAware = aware; });
        }

        [Fact]
        public void Execute_ShouldCallInitializerOnce()
        {
            this.testee.Execute(new List<ICustomExtension> { A.Fake<ICustomExtension>(), A.Fake<ICustomExtension>() }, this.executableContext);

            this.contextAccessCounter.Should().Be(1);
        }

        [Fact]
        public void Execute_ShouldPassItselfToInitializer()
        {
            this.testee.Execute(new List<ICustomExtension> { A.Fake<ICustomExtension>(), A.Fake<ICustomExtension>() }, this.executableContext);

            this.interceptedBehaviorAware.Should().Be(this.testee);
        }

        [Fact]
        public void Execute_ShouldExecuteActionOnExtensions()
        {
            var firstExtension = A.Fake<ICustomExtension>();
            var secondExtension = A.Fake<ICustomExtension>();

            this.testee.Execute(new List<ICustomExtension> { firstExtension, secondExtension }, this.executableContext);

            A.CallTo(() => firstExtension.SomeMethod(A<object>._)).MustHaveHappened();
            A.CallTo(() => secondExtension.SomeMethod(A<object>._)).MustHaveHappened();
        }

        [Fact]
        public void Execute_ShouldPassContextToExtensions()
        {
            var firstExtension = A.Fake<ICustomExtension>();
            var secondExtension = A.Fake<ICustomExtension>();

            this.testee.Execute(new List<ICustomExtension> { firstExtension, secondExtension }, this.executableContext);

            A.CallTo(() => firstExtension.SomeMethod(this.context)).MustHaveHappened();
            A.CallTo(() => secondExtension.SomeMethod(this.context)).MustHaveHappened();
        }

        [Fact]
        public void ShouldDescribeItself()
        {
            this.testee.Describe().Should().Be("Initializes the context once with \"() => value(Appccelerate.Bootstrapper.Syntax.Executables.ActionOnExtensionWithInitializerExecutableTest).CountAccessToContext()\" and executes \"(x, i) => x.SomeMethod(i)\" on each extension during bootstrapping.");
        }

        private object CountAccessToContext()
        {
            this.contextAccessCounter++;
            return this.context;
        }
    }
}