//-------------------------------------------------------------------------------
// <copyright file="HaveConfigurationSectionNameTest.cs" company="Appccelerate">
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

namespace Appccelerate.Bootstrapper.Configuration
{
    using Appccelerate.Bootstrapper.Configuration.Internals;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class HaveConfigurationSectionNameTest
    {
        private const string AnyName = "AnyName";

        [Fact]
        public void SectionName_ExtensionNotIHaveExtensionConfigurationSectionName_ShouldUseTypeName()
        {
            var extension = A.Fake<IExtension>();
            var expected = extension.GetType().Name;

            var testee = new HaveConfigurationSectionName(extension);
            testee.SectionName.Should().Be(expected);
        }

        [Fact]
        public void SectionName_ExtensionIHaveExtensionConfigurationSectionName_ShouldAcquireNameFromExtension()
        {
            var extension = A.Fake<IExtension>(builder => builder.Implements(typeof(IHaveConfigurationSectionName)));
            var namer = extension as IHaveConfigurationSectionName;
            A.CallTo(() => namer.SectionName).Returns(AnyName);

            var testee = new HaveConfigurationSectionName(extension);
            testee.SectionName.Should().Be(AnyName);
        }
    }
}