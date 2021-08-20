//-------------------------------------------------------------------------------
// <copyright file="when_the_bootstrapper_is_run_with_behavior_attached.cs" company="Appccelerate">
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

    public class WhenTheBootstrapperIsRunWithBehaviorAttached
    {
        private readonly Queue<string> sequenceQueue;
        private readonly CustomExtensionWithBehaviorStrategy strategy;
        private readonly FirstExtension first;
        private readonly SecondExtension second;

        public WhenTheBootstrapperIsRunWithBehaviorAttached()
        {
            sequenceQueue = new Queue<string>();
            strategy = new CustomExtensionWithBehaviorStrategy(sequenceQueue);
            first = new FirstExtension(sequenceQueue);
            second = new SecondExtension(sequenceQueue);

            var bootstrapper = new DefaultBootstrapper<ICustomExtension>();
            bootstrapper.Initialize(strategy);
            bootstrapper.AddExtension(first);
            bootstrapper.AddExtension(second);
            bootstrapper.Run();
        }

        [Fact]
        public void should_only_initialize_contexts_once_for_all_extensions()
        {
            strategy.RunConfigurationInitializerAccessCounter.Should().Be(1);
        }

        [Fact]
        public void should_pass_the_initialized_values_from_the_contexts_to_the_extensions()
        {
            var expected = new Dictionary<string, string>
            {
                { "RunTest", "RunTestValue" },
                { "RunFirstValue", "RunTestValue" },
                { "RunSecondValue", "RunTestValue" },
            };

            first.RunConfiguration.Should().Equal(expected);
            second.RunConfiguration.Should().Equal(expected);

            first.Registered.Should().Be("RunTest");
            second.Registered.Should().Be("RunTest");
        }

        [Fact]
        public void should_execute_the_extensions_with_its_extension_points_and_the_behaviors_according_to_the_strategy_defined_order()
        {
            sequenceQueue.Should().HaveCount(33, sequenceQueue.Flatten());
            sequenceQueue.Should().BeEquivalentTo(new[]
            {
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at run first beginning.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at run first beginning.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at run second beginning.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at run second beginning.",

                "Action: CustomRun",

                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at run first start.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at run first start.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at run second start.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at run second start.",
                "FirstExtension: Start",
                "SecondExtension: Start",

                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at configuration modification with RunFirstValue = RunTestValue.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at configuration modification with RunFirstValue = RunTestValue.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at configuration modification with RunSecondValue = RunTestValue.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at configuration modification with RunSecondValue = RunTestValue.",
                "FirstExtension: Configure",
                "SecondExtension: Configure",

                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at run first initialize.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at run first initialize.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at run second initialize.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at run second initialize.",
                "FirstExtension: Initialize",
                "SecondExtension: Initialize",

                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at input modification with RunTestValueFirst.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at input modification with RunTestValueFirst.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at input modification with RunTestValueSecond.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at input modification with RunTestValueSecond.",
                "FirstExtension: Register",
                "SecondExtension: Register",

                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at run first end.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at run first end.",
                "FirstExtension: Behaving on Appccelerate.Bootstrapper.Dummies.FirstExtension at run second end.",
                "SecondExtension: Behaving on Appccelerate.Bootstrapper.Dummies.SecondExtension at run second end.",
            });
        }
    }
}