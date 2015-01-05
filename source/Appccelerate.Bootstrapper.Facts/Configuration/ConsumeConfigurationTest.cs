//-------------------------------------------------------------------------------
// <copyright file="ConsumeConfigurationTest.cs" company="Appccelerate">
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

namespace Appccelerate.Bootstrapper.Configuration
{
    using System.Collections.Generic;

    using Appccelerate.Bootstrapper.Configuration.Internals;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ConsumeConfigurationTest
    {
        [Fact]
        public void Configuration_ExtensionNotIConsumeConfiguration_ShouldUseEmtpyOne()
        {
            var extension = A.Fake<IExtension>();

            var testee = new ConsumeConfiguration(extension);
            testee.Configuration.Should().BeEmpty();
        }

        [Fact]
        public void Configuration_ExtensionIHaveExtensionConfigurationSectionName_ShouldAcquireNameFromExtension()
        {
            var extension = A.Fake<IExtension>(builder => builder.Implements(typeof(IConsumeConfiguration)));
            var consumer = extension as IConsumeConfiguration;
            var expected = new KeyValuePair<string, string>("Value", "Key");

            A.CallTo(() => consumer.Configuration)
                .Returns(new Dictionary<string, string> { { expected.Key, expected.Value } });

            var testee = new ConsumeConfiguration(extension);
            testee.Configuration.Should().Contain(expected);
        }
    }
}