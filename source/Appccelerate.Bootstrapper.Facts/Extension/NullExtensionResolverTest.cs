//-------------------------------------------------------------------------------
// <copyright file="NullExtensionResolverTest.cs" company="Appccelerate">
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

namespace Appccelerate.Bootstrapper.Extension
{
    using FakeItEasy;
    using Xunit;

    public class NullExtensionResolverTest
    {
        private readonly IExtensionPoint<IExtension> extensionPoint;

        private readonly NullExtensionResolver<IExtension> testee;

        public NullExtensionResolverTest()
        {
            this.extensionPoint = A.Fake<IExtensionPoint<IExtension>>();

            this.testee = new NullExtensionResolver<IExtension>();
        }

        [Fact]
        public void Resolve_ShouldNotAddExtensions()
        {
            this.testee.Resolve(this.extensionPoint);

            A.CallTo(() => this.extensionPoint.AddExtension(A<IExtension>._)).MustNotHaveHappened();
        }
    }
}