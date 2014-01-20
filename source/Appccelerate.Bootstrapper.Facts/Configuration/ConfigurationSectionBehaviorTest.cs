//-------------------------------------------------------------------------------
// <copyright file="ConfigurationSectionBehaviorTest.cs" company="Appccelerate">
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

namespace Appccelerate.Bootstrapper.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;
    using Appccelerate.Formatters;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ConfigurationSectionBehaviorTest
    {
        private readonly IConsumeConfigurationSection consumer;

        private readonly IHaveConfigurationSectionName sectionNameProvider;

        private readonly ILoadConfigurationSection sectionProvider;

        private readonly IConfigurationSectionBehaviorFactory factory;

        private readonly List<IExtension> extensions;

        private readonly ConfigurationSectionBehavior testee;

        public ConfigurationSectionBehaviorTest()
        {
            this.consumer = A.Fake<IConsumeConfigurationSection>();
            this.sectionNameProvider = A.Fake<IHaveConfigurationSectionName>();
            this.sectionProvider = A.Fake<ILoadConfigurationSection>();

            this.factory = A.Fake<IConfigurationSectionBehaviorFactory>();
            this.AutoStubFactory();

            this.extensions = new List<IExtension> { A.Fake<IExtension>(), A.Fake<IExtension>(), };

            this.testee = new ConfigurationSectionBehavior(this.factory);
        }

        [Fact]
        public void Behave_ShouldApply()
        {
            this.testee.Behave(this.extensions);

            A.CallTo(() => this.consumer.Apply(A<ConfigurationSection>._)).MustHaveHappened();
        }

        [Fact]
        public void Behave_ShouldApplySectionFromProvider()
        {
            var configurationSection = A.Fake<ConfigurationSection>();
            A.CallTo(() => this.sectionProvider.GetSection(A<string>._)).Returns(configurationSection);

            this.testee.Behave(this.extensions);

            A.CallTo(() => this.consumer.Apply(configurationSection)).MustHaveHappened();
        }

        [Fact]
        public void Behave_ShouldAcquireSectionName()
        {
            this.testee.Behave(this.extensions);

            A.CallTo(() => this.sectionNameProvider.SectionName).MustHaveHappened();
        }

        [Fact]
        public void Behave_ShouldAcquireSectionByName()
        {
            const string AnySectionName = "SectionName";

            A.CallTo(() => this.sectionNameProvider.SectionName).Returns(AnySectionName);

            this.testee.Behave(this.extensions);

            A.CallTo(() => this.sectionProvider.GetSection(AnySectionName)).MustHaveHappened();
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
            this.testee.Describe().Should().Be("Automatically provides configuration sections for all extensions.");
        }

        private void AutoStubFactory()
        {
            A.CallTo(() => this.factory.CreateConsumeConfigurationSection(A<IExtension>._)).Returns(this.consumer);
            A.CallTo(() => this.factory.CreateHaveConfigurationSectionName(A<IExtension>._)).Returns(this.sectionNameProvider);
            A.CallTo(() => this.factory.CreateLoadConfigurationSection(A<IExtension>._)).Returns(this.sectionProvider);
        }
    }
}