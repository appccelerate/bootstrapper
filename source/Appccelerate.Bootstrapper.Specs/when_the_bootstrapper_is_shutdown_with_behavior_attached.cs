//-------------------------------------------------------------------------------
// <copyright file="when_the_bootstrapper_is_shutdown_with_behavior_attached.cs" company="Appccelerate">
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

    public class WhenTheBootstrapperIsShutdownWithBehaviorAttached
    {
        private readonly Queue<string> sequenceQueue;
        private readonly CustomExtensionWithBehaviorStrategy strategy;
        private readonly FirstExtension first;
        private readonly SecondExtension second;

        public WhenTheBootstrapperIsShutdownWithBehaviorAttached()
        {
            sequenceQueue = new Queue<string>();
            strategy = new CustomExtensionWithBehaviorStrategy(sequenceQueue);
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
                { "ShutdownTest", "ShutdownTestValue" },
                { "ShutdownFirstValue", "ShutdownTestValue" },
                { "ShutdownSecondValue", "ShutdownTestValue" },
            };

            first.ShutdownConfiguration.Should().Equal(expected);
            second.ShutdownConfiguration.Should().Equal(expected);

            first.Unregistered.Should().Be("ShutdownTest");
            second.Unregistered.Should().Be("ShutdownTest");
        }

        [Fact]
        public void should_execute_the_extensions_with_its_extension_points_and_the_behaviors_according_to_the_strategy_defined_order()
        {
            sequenceQueue.Should().HaveCount(29, sequenceQueue.Flatten());
            sequenceQueue.Should().BeEquivalentTo(new[]
            {
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at shutdown first beginning.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at shutdown first beginning.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at shutdown second beginning.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at shutdown second beginning.",

                "Action: CustomShutdown",

                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at input modification with ShutdownTestValueFirst.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at input modification with ShutdownTestValueFirst.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at input modification with ShutdownTestValueSecond.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at input modification with ShutdownTestValueSecond.",
                "SecondExtension: Unregister",
                "FirstExtension: Unregister",

                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at configuration modification with ShutdownFirstValue = ShutdownTestValue.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at configuration modification with ShutdownFirstValue = ShutdownTestValue.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at configuration modification with ShutdownSecondValue = ShutdownTestValue.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at configuration modification with ShutdownSecondValue = ShutdownTestValue.",
                "SecondExtension: DeConfigure",
                "FirstExtension: DeConfigure",

                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at shutdown first stop.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at shutdown first stop.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at shutdown second stop.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at shutdown second stop.",
                "SecondExtension: Stop",
                "FirstExtension: Stop",

                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at shutdown first end.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at shutdown first end.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at shutdown second end.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at shutdown second end.",
                "SecondExtension: Dispose",
                "FirstExtension: Dispose"
            });
        }
    }
}