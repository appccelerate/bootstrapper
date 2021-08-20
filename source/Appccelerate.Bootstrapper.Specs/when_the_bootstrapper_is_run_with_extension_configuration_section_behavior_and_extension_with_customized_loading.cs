//-------------------------------------------------------------------------------
// <copyright file="when_the_bootstrapper_is_run_with_extension_configuration_section_behavior_and_extension_with_customized_loading.cs" company="Appccelerate">
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

namespace Appccelerate.Bootstrapper
{
    using Dummies;
    using FluentAssertions;
    using Xunit;

    public class WhenTheBootstrapperIsRunWithExtensionConfigurationSectionBehaviorAndExtensionWithCustomizedLoading
    {
        private readonly CustomExtensionWithExtensionConfigurationWhichHasCallbacks withCallbacksExtension;
        private readonly CustomExtensionWithExtensionConfigurationWhichConsumesConfiguration whichConsumesConfigurationExtension;

        public WhenTheBootstrapperIsRunWithExtensionConfigurationSectionBehaviorAndExtensionWithCustomizedLoading()
        {
            var strategy = new CustomExtensionWithExtensionConfigurationStrategy();
            withCallbacksExtension = new CustomExtensionWithExtensionConfigurationWhichHasCallbacks();
            whichConsumesConfigurationExtension = new CustomExtensionWithExtensionConfigurationWhichConsumesConfiguration();

            IBootstrapper<ICustomExtensionWithExtensionConfiguration> bootstrapper = 
                new DefaultBootstrapper<ICustomExtensionWithExtensionConfiguration>();
            bootstrapper.Initialize(strategy);
            bootstrapper.AddExtension(withCallbacksExtension);
            bootstrapper.AddExtension(whichConsumesConfigurationExtension);
            bootstrapper.Run();
        }

        [Fact]
        public void should_use_default_conversion_callback()
        {
            withCallbacksExtension.SomeStringWithDefault.Should().Be("SomeStringWithDefault. Modified by Default!");
        }

        [Fact]
        public void should_use_conversion_callbacks()
        {
            withCallbacksExtension.SomeString.Should().Be("SomeString. Modified by Callback!");
            withCallbacksExtension.SomeInt.Should().Be(1);
        }

        [Fact]
        public void should_ignore_not_configured_properties()
        {
            withCallbacksExtension.SomeStringWhichIsIgnored.Should().BeNull();
        }

        [Fact]
        public void should_propagate_configuration()
        {
            whichConsumesConfigurationExtension.Configuration.Should()
                .HaveCount(3)
                .And.Contain("SomeInt", "1")
                .And.Contain("SomeString", "SomeString")
                .And.Contain("SomeStringWithDefault", "SomeStringWithDefault");
        }

        [Fact]
        public void should_acquire_section()
        {
            withCallbacksExtension.SectionAcquired.Should()
                .Be("CustomExtensionWithExtensionConfigurationWhichHasCallbacks");
            whichConsumesConfigurationExtension.SectionAcquired.Should()
                .Be("CustomExtensionWithExtensionConfigurationWhichConsumesConfiguration");
        }
    }
}