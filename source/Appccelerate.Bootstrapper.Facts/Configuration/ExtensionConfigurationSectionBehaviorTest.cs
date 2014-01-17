//-------------------------------------------------------------------------------
// <copyright file="ExtensionConfigurationSectionBehaviorTest.cs" company="Appccelerate">
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
    using Appccelerate.Formatters;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ExtensionConfigurationSectionBehaviorTest
    {
        private readonly IExtensionConfigurationSectionBehaviorFactory factory;
        private readonly IHaveConversionCallbacks conversionCallbacksProvider;
        private readonly ILoadConfigurationSection sectionProvider;
        private readonly IConsumeConfiguration consumer;
        private readonly IHaveConfigurationSectionName sectionNameProvider;
        private readonly IReflectExtensionProperties extensionPropertyReflector;
        private readonly IAssignExtensionProperties assigner;
        private readonly IHaveDefaultConversionCallback defaultConversionCallbackProvider;
        private readonly List<IExtension> extensions;

        private readonly ExtensionConfigurationSectionBehavior testee;

        public ExtensionConfigurationSectionBehaviorTest()
        {
            this.consumer = A.Fake<IConsumeConfiguration>();
            this.extensionPropertyReflector = A.Fake<IReflectExtensionProperties>();
            this.sectionNameProvider = A.Fake<IHaveConfigurationSectionName>();
            this.sectionProvider = A.Fake<ILoadConfigurationSection>();
            this.conversionCallbacksProvider = A.Fake<IHaveConversionCallbacks>();
            this.defaultConversionCallbackProvider = A.Fake<IHaveDefaultConversionCallback>();
            this.assigner = A.Fake<IAssignExtensionProperties>();

            this.factory = A.Fake<IExtensionConfigurationSectionBehaviorFactory>();
            this.SetupAutoStubFactory();

            this.extensions = new List<IExtension> { A.Fake<IExtension>(), };

            this.testee = new ExtensionConfigurationSectionBehavior(this.factory);
        }

        [Fact]
        public void Behave_ShouldConsumeSectionFromProvider()
        {
            var expectedConfiguration = new KeyValuePair<string, string>("AnyKey", "AnyValue");
            var configuration = new Dictionary<string, string>();

            var configurationSection = ExtensionConfigurationSectionHelper.CreateSection(expectedConfiguration);

            A.CallTo(() => this.sectionProvider.GetSection(A<string>._)).Returns(configurationSection);
            A.CallTo(() => this.consumer.Configuration).Returns(configuration);

            this.testee.Behave(this.extensions);

            configuration.Should().Contain(expectedConfiguration);
        }

        [Fact]
        public void Behave_ShouldAcquireSectionName()
        {
            this.SetupEmptyConsumerConfiguration();

            this.testee.Behave(this.extensions);

            A.CallTo(() => this.sectionNameProvider.SectionName).MustHaveHappened();
        }

        [Fact]
        public void Behave_ShouldAcquireSectionByName()
        {
            this.SetupEmptyConsumerConfiguration();

            const string AnySectionName = "SectionName";

            A.CallTo(() => this.sectionNameProvider.SectionName).Returns(AnySectionName);

            this.testee.Behave(this.extensions);

            A.CallTo(() => this.sectionProvider.GetSection(AnySectionName)).MustHaveHappened();
        }

        [Fact]
        public void Behave_ShouldAssign()
        {
            this.SetupEmptyConsumerConfiguration();
            this.SetupExtensionConfigurationSectionWithEntries();

            this.testee.Behave(this.extensions);

            A.CallTo(() => this.assigner.Assign(this.extensionPropertyReflector, A<IExtension>._, this.consumer, this.conversionCallbacksProvider, this.defaultConversionCallbackProvider))
                .MustHaveHappened();
        }

        [Fact]
        public void Behave_ShouldNotProceedWhenNoConfigurationAvailable()
        {
            var configurationSection = ExtensionConfigurationSectionHelper.CreateSection(new Dictionary<string, string>());

            A.CallTo(() => this.sectionProvider.GetSection(A<string>._)).Returns(configurationSection);

            this.testee.Behave(this.extensions);

            A.CallTo(() => this.consumer.Configuration).MustNotHaveHappened();
            A.CallTo(() => this.assigner.Assign(A<IReflectExtensionProperties>._, A<IExtension>._, A<IConsumeConfiguration>._, A<IHaveConversionCallbacks>._, A<IHaveDefaultConversionCallback>._))
                .MustNotHaveHappened();
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
            this.testee.Describe().Should().Be("Automatically propagates properties of all extensions with configuration values when a matching ExtensionConfigurationSection is found.");
        }

        private void SetupExtensionConfigurationSectionWithEntries()
        {
            var configurationSection = ExtensionConfigurationSectionHelper.CreateSection(new Dictionary<string, string> { { "AnyKey", "AnyValue" } });
            A.CallTo(() => this.sectionProvider.GetSection(A<string>._)).Returns(configurationSection);
        }

        private void SetupEmptyConsumerConfiguration()
        {
            A.CallTo(() => this.consumer.Configuration).Returns(new Dictionary<string, string>());
        }

        private void SetupAutoStubFactory()
        {
            A.CallTo(() => this.factory.CreateConsumeConfiguration(A<IExtension>._)).Returns(this.consumer);
            A.CallTo(() => this.factory.CreateReflectExtensionProperties()).Returns(this.extensionPropertyReflector);
            A.CallTo(() => this.factory.CreateAssignExtensionProperties()).Returns(this.assigner);
            A.CallTo(() => this.factory.CreateHaveConfigurationSectionName(A<IExtension>._)).Returns(this.sectionNameProvider);
            A.CallTo(() => this.factory.CreateHaveConversionCallbacks(A<IExtension>._)).Returns(this.conversionCallbacksProvider);
            A.CallTo(() => this.factory.CreateHaveDefaultConversionCallback(A<IExtension>._)).Returns(this.defaultConversionCallbackProvider);
            A.CallTo(() => this.factory.CreateLoadConfigurationSection(A<IExtension>._)).Returns(this.sectionProvider);
        }
    }
}