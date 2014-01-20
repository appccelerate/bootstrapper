//-------------------------------------------------------------------------------
// <copyright file="ExecutionContextTest.cs" company="Appccelerate">
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

namespace Appccelerate.Bootstrapper.Reporting
{
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ExecutionContextTest
    {
        private readonly IDescribable describable;

        public ExecutionContextTest()
        {
            this.describable = A.Fake<IDescribable>();
        }

        [Fact]
        public void Constructor_ShouldDescribe()
        {
            const string ExpectedName = "Name";
            const string ExpectedDescription = "TestDescription";

            A.CallTo(() => this.describable.Name).Returns(ExpectedName);
            A.CallTo(() => this.describable.Describe()).Returns(ExpectedDescription);

            ExecutionContext testee = CreateTestee(this.describable);

            testee.Name.Should().Be(ExpectedName);
            testee.Description.Should().Be(ExpectedDescription);
            A.CallTo(() => this.describable.Describe()).MustHaveHappened();
        }

        [Fact]
        public void Constructor_Executables_ShouldBeEmpty()
        {
            ExecutionContext testee = CreateTestee(A.Fake<IDescribable>());

            testee.Executables.Should().BeEmpty();
        }

        [Fact]
        public void CreateExecutableContext_ShouldCreateExecutableContext()
        {
            ExecutionContext testee = CreateTestee(A.Fake<IDescribable>());

            var executableContext = testee.CreateExecutableContext(A.Fake<IDescribable>());

            testee.Executables.Should().NotBeEmpty()
                .And.HaveCount(1)
                .And.Contain(executableContext);
        }

        private static ExecutionContext CreateTestee(IDescribable describable)
        {
            return new ExecutionContext(describable);
        }
    }
}