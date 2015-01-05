//-------------------------------------------------------------------------------
// <copyright file="SynchronousExecutorTest.cs" company="Appccelerate">
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

namespace Appccelerate.Bootstrapper.Execution
{
    using System.Collections.Generic;
    using System.Linq;
    using Appccelerate.Bootstrapper.Reporting;
    using Appccelerate.Bootstrapper.Syntax;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class SynchronousExecutorTest
    {
        private readonly IExecutionContext executionContext;

        private readonly SynchronousExecutor<IExtension> testee;

        public SynchronousExecutorTest()
        {
            this.executionContext = A.Fake<IExecutionContext>();

            this.testee = new SynchronousExecutor<IExtension>();
        }

        [Fact]
        public void Execute_ShouldExecuteSyntaxWithExtensionsInOrderOfAppearance()
        {
            var executable = A.Fake<IExecutable<IExtension>>();
            var syntax = A.Fake<ISyntax<IExtension>>();
            var firstExtension = A.Fake<IExtension>();
            var secondExtension = A.Fake<IExtension>();

            var extensions = new List<IExtension> { secondExtension, firstExtension, };

            IEnumerable<IExtension> passedExtensions = Enumerable.Empty<IExtension>();

            A.CallTo(() => executable.Execute(A<IEnumerable<IExtension>>._, A<IExecutableContext>._))
                .Invokes((IEnumerable<IExtension> ext, IExecutableContext ctx) => passedExtensions = ext);

            A.CallTo(() => syntax.GetEnumerator())
                .Returns(new List<IExecutable<IExtension>> { executable }
                .GetEnumerator());

            this.testee.Execute(syntax, extensions, this.executionContext);

            passedExtensions.Should().ContainInOrder(extensions);
        }

        [Fact]
        public void ShouldDescribeItself()
        {
            this.testee.Describe().Should().Be("Runs all executables synchronously on the extensions in the order which they were added.");
        }
    }
}