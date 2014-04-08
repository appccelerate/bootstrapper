//-------------------------------------------------------------------------------
// <copyright file="ConsumeConfigurationSectionTest.cs" company="Appccelerate">
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
    using Xunit;

    public class ConsumeConfigurationSectionTest
    {
        [Fact]
        public void Apply_WhenExtensionIConsumeConfigurationSection_ShouldApplySection()
        {
            var extension = A.Fake<IExtension>(builder => builder.Implements(typeof(IConsumeConfigurationSection)));
            var consumer = extension as IConsumeConfigurationSection;

            var testee = new ConsumeConfigurationSection(extension);
            testee.Apply(null);

            A.CallTo(() => consumer.Apply(null)).MustHaveHappened();
        }

        [Fact]
        public void Apply_WhenExtensionNotIConsumeConfigurationSection_ShouldNotApplySection()
        {
            var extension = A.Fake<IExtension>();
            var testee = new ConsumeConfigurationSection(extension);
            testee.Apply(null);

            A.CallTo(extension).MustNotHaveHappened();
        }
    }
}