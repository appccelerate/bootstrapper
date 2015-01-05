//-------------------------------------------------------------------------------
// <copyright file="DisposeExtensionBehaviorTest.cs" company="Appccelerate">
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

namespace Appccelerate.Bootstrapper.Behavior
{
    using System.Collections.Generic;
    using Appccelerate.Bootstrapper.Dummies;
    using Appccelerate.Formatters;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class DisposeExtensionBehaviorTest
    {
        private readonly DisposeExtensionBehavior testee;

        public DisposeExtensionBehaviorTest()
        {
            this.testee = new DisposeExtensionBehavior();
        }

        [Fact]
        public void Behave_ShouldDisposeDisposableExtensions()
        {
            var notDisposableExtension = A.Fake<INonDisposableExtension>();
            var disposableExtension = A.Fake<IDisposableExtension>();

            this.testee.Behave(new List<IExtension> { notDisposableExtension, disposableExtension });

            A.CallTo(() => notDisposableExtension.Dispose()).MustNotHaveHappened();
            A.CallTo(() => disposableExtension.Dispose()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ShouldReturnTypeName()
        {
            string expectedName = this.testee.GetType().FullNameToString();

            this.testee.Name.Should().Be(expectedName);
        }

        [Fact]
        public void ShouldDescribeItself()
        {
            this.testee.Describe().Should().Be("Disposes all extensions which implement IDisposable.");
        }
    }
}