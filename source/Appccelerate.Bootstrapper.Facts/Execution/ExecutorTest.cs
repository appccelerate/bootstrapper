//-------------------------------------------------------------------------------
// <copyright file="ExecutorTest.cs" company="Appccelerate">
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

using Xunit;

namespace Appccelerate.Bootstrapper.Execution
{
    using System.Collections.Generic;
    using Appccelerate.Bootstrapper.Reporting;
    using Appccelerate.Bootstrapper.Syntax;
    using Appccelerate.Formatters;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit.Extensions;

    public class ExecutorTest
    {
        private readonly IExecutionContext executionContext;
        private readonly IExecutable<IExtension> firstExecutable;
        private readonly IExecutable<IExtension> secondExecutable;
        private readonly ISyntax<IExtension> syntax;
        private readonly List<IExecutable<IExtension>> executables;

        private readonly List<IExtension> extensions;

        public ExecutorTest()
        {
            this.firstExecutable = A.Fake<IExecutable<IExtension>>();
            this.secondExecutable = A.Fake<IExecutable<IExtension>>();

            this.executionContext = A.Fake<IExecutionContext>();
            this.executables = new List<IExecutable<IExtension>> { this.firstExecutable, this.secondExecutable };
            this.syntax = A.Fake<ISyntax<IExtension>>();

            this.extensions = new List<IExtension> { A.Fake<IExtension>(), };
        }

        public static IEnumerable<object[]> Testees
        {
            get
            {
                yield return new object[] { new SynchronousExecutor<IExtension>() };
                yield return new object[] { new SynchronousReverseExecutor<IExtension>() };
            }
        }

        [Theory]
        [MemberData(nameof(Testees))]
        public void Execute_ShouldExecuteSyntaxWithExtensions(IExecutor<IExtension> testee)
        {
            this.SetupSyntaxReturnsExecutables();

            testee.Execute(this.syntax, this.extensions, this.executionContext);

            A.CallTo(() => this.firstExecutable.Execute(A<IEnumerable<IExtension>>._, A<IExecutableContext>._)).MustHaveHappened();
            A.CallTo(() => this.secondExecutable.Execute(A<IEnumerable<IExtension>>._, A<IExecutableContext>._)).MustHaveHappened();
        }

        [Theory]
        [MemberData(nameof(Testees))]
        public void Execute_ShouldCreateExecutableContextForExecutables(IExecutor<IExtension> testee)
        {
            this.SetupSyntaxReturnsExecutables();

            testee.Execute(this.syntax, this.extensions, this.executionContext);

            A.CallTo(() => this.executionContext.CreateExecutableContext(this.firstExecutable)).MustHaveHappened();
            A.CallTo(() => this.executionContext.CreateExecutableContext(this.secondExecutable)).MustHaveHappened();
        }

        [Theory]
        [MemberData(nameof(Testees))]
        public void Execute_ShouldProvideExecutableContextForExecutables(IExecutor<IExtension> testee)
        {
            this.SetupSyntaxReturnsExecutables();

            var firstExecutableContext = A.Fake<IExecutableContext>();
            var secondExecutableContext = A.Fake<IExecutableContext>();

            A.CallTo(() => this.executionContext.CreateExecutableContext(this.firstExecutable)).Returns(firstExecutableContext);
            A.CallTo(() => this.executionContext.CreateExecutableContext(this.secondExecutable)).Returns(secondExecutableContext);

            testee.Execute(this.syntax, this.extensions, this.executionContext);

            A.CallTo(() => this.firstExecutable.Execute(A<IEnumerable<IExtension>>._, firstExecutableContext)).MustHaveHappened();
            A.CallTo(() => this.secondExecutable.Execute(A<IEnumerable<IExtension>>._, secondExecutableContext)).MustHaveHappened();
        }

        [Theory]
        [MemberData(nameof(Testees))]
        public void Name_ShouldReturnTypeName(IExecutor<IExtension> testee)
        {
            string expectedName = testee.GetType().FullNameToString();

            testee.Name.Should().Be(expectedName);
        }

        private void SetupSyntaxReturnsExecutables()
        {
            A.CallTo(() => this.syntax.GetEnumerator()).Returns(this.executables.GetEnumerator());
        }
    }
}