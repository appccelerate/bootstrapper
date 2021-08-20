//-------------------------------------------------------------------------------
// <copyright file="when_the_bootstrapper_is_shutdown.cs" company="Appccelerate">
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
    using System.Collections.Generic;
    using FluentAssertions;
    using Xunit;

    public class WhenTheBootstrapperIsShutdown
    {
        private readonly Queue<string> sequenceQueue;
        private readonly CustomExtensionStrategy strategy;
        private readonly FirstExtension first;
        private readonly SecondExtension second;

        public WhenTheBootstrapperIsShutdown()
        {
            sequenceQueue = new Queue<string>();
            strategy = new CustomExtensionStrategy(sequenceQueue);
            first = new FirstExtension(sequenceQueue);
            second = new SecondExtension(sequenceQueue);

            var bootstrapper = new DefaultBootstrapper<ICustomExtension>();
            bootstrapper.Initialize(strategy);
            bootstrapper.AddExtension(first);
            bootstrapper.AddExtension(second);
            bootstrapper.Shutdown();
        }

        [Fact]
        public void should_only_initialize_contexts_once_for_all_extensions()
        {
            strategy.ShutdownConfigurationInitializerAccessCounter.Should().Be(1);
        }

        [Fact]
        public void should_pass_the_initialized_values_from_the_contexts_to_the_extensions()
        {
            var expected = new Dictionary<string, string>
            {
                { "ShutdownTest", "ShutdownTestValue" }
            };

            first.ShutdownConfiguration.Should().Equal(expected);
            second.ShutdownConfiguration.Should().Equal(expected);

            first.Unregistered.Should().Be("ShutdownTest");
            second.Unregistered.Should().Be("ShutdownTest");
        }

        [Fact]
        public void should_execute_the_extensions_and_the_extension_points_according_to_the_strategy_defined_order()
        {
            sequenceQueue.Should().HaveCount(7, sequenceQueue.Flatten());
            sequenceQueue.Should().BeEquivalentTo(new[]
            {
                "Action: CustomShutdown",

                "SecondExtension: Unregister",
                "FirstExtension: Unregister",

                "SecondExtension: DeConfigure",
                "FirstExtension: DeConfigure",

                "SecondExtension: Stop",
                "FirstExtension: Stop"
            });
        }
    }
}