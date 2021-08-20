//-------------------------------------------------------------------------------
// <copyright file="when_the_bootstrapper_is_run_with_configuration_section_behavior_and_extension_with_customized_loading.cs" company="Appccelerate">
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

    public class WhenTheBootstrapperIsRunWithConfigurationSectionBehaviorAndExtensionWithCustomizedLoading
    {
        private readonly CustomExtensionWithConfigurationWhichKnowsNameAndWhereToLoadFrom nameAndWhereToLoadFromExtension;
        private readonly CustomExtensionWithConfigurationWhichKnowsWhereToLoadFrom whereToLoadFromExtension;

        public WhenTheBootstrapperIsRunWithConfigurationSectionBehaviorAndExtensionWithCustomizedLoading()
        {
            nameAndWhereToLoadFromExtension = new CustomExtensionWithConfigurationWhichKnowsNameAndWhereToLoadFrom();
            whereToLoadFromExtension = new CustomExtensionWithConfigurationWhichKnowsWhereToLoadFrom();

            var bootstrapper = new DefaultBootstrapper<ICustomExtensionWithConfiguration>();
            var strategy = new CustomExtensionWithConfigurationStrategy();
            bootstrapper.Initialize(strategy);
            bootstrapper.AddExtension(nameAndWhereToLoadFromExtension);
            bootstrapper.AddExtension(whereToLoadFromExtension);
            bootstrapper.Run();
        }

        [Fact]
        public void should_apply_configuration_section()
        {
            nameAndWhereToLoadFromExtension.AppliedSection.Should().NotBeNull();
            nameAndWhereToLoadFromExtension.AppliedSection.Context.Should().Be("KnowsName|KnowsLoading");

            whereToLoadFromExtension.AppliedSection.Should().NotBeNull();
            whereToLoadFromExtension.AppliedSection.Context.Should().Be("KnowsLoading");
        }

        [Fact]
        public void should_acquire_section_name()
        {
            nameAndWhereToLoadFromExtension.SectionNameAcquired.Should().BeTrue();
        }

        [Fact]
        public void should_acquire_section()
        {
            nameAndWhereToLoadFromExtension.SectionAcquired.Should().Be("FakeConfigurationSection");
            whereToLoadFromExtension.SectionAcquired.Should().Be("CustomExtensionWithConfigurationWhichKnowsWhereToLoadFrom");
        }
    }
}