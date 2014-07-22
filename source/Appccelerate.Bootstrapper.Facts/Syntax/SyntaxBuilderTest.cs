//-------------------------------------------------------------------------------
// <copyright file="SyntaxBuilderTest.cs" company="Appccelerate">
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

namespace Appccelerate.Bootstrapper.Syntax
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using Appccelerate.Bootstrapper.Dummies;
    using Appccelerate.Formatters;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;
    using Xunit.Extensions;

    public class SyntaxBuilderTest
    {
        private const int NumberOfExecutablesForBegin = 1;

        private const int NumberOfExecutablesForEnd = 2;

        private readonly StringBuilder executionChainingBuilder;

        private readonly IExecutableFactory<ICustomExtension> executableFactory;

        private readonly ISyntaxBuilder<ICustomExtension> testee;

        public SyntaxBuilderTest()
        {
            this.executionChainingBuilder = new StringBuilder();
            this.executableFactory = A.Fake<IExecutableFactory<ICustomExtension>>();

            this.testee = new SyntaxBuilder<ICustomExtension>(this.executableFactory);
        }

        [Fact]
        public void BeginWith_Behavior_ShouldAddExecutable()
        {
            this.SetupCreateActionExecutableReturnsAnyExecutable();

            this.testee.Begin.With(A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(NumberOfExecutablesForBegin);
        }

        [Fact]
        public void BeginWith_BehaviorMultipleTimes_ShouldOnlyAddOneExecutable()
        {
            this.SetupCreateActionExecutableReturnsAnyExecutable();

            this.testee.Begin.With(A.Fake<IBehavior<ICustomExtension>>()).With(A.Fake<IBehavior<ICustomExtension>>()).With(A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(NumberOfExecutablesForBegin);
        }

        [Fact]
        public void BeginWith_Behavior_ShouldAddBehaviorToLastExecutable()
        {
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var behavior = A.Fake<IBehavior<ICustomExtension>>();

            this.SetupCreateActionExecutableReturnsExecutable(extension);

            this.testee.Begin.With(behavior);

            A.CallTo(() => extension.Add(behavior)).MustHaveHappened();
        }

        [Fact]
        public void BeginWith_BehaviorMultipleTimes_ShouldAddBehaviorToLastExecutable()
        {
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var firstBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var secondBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var thirdBehavior = A.Fake<IBehavior<ICustomExtension>>();

            this.SetupCreateActionExecutableReturnsExecutable(extension);

            this.testee
                .Begin
                    .With(firstBehavior)
                    .With(secondBehavior)
                    .With(thirdBehavior);

            A.CallTo(() => extension.Add(firstBehavior)).MustHaveHappened();
            A.CallTo(() => extension.Add(secondBehavior)).MustHaveHappened();
            A.CallTo(() => extension.Add(thirdBehavior)).MustHaveHappened();
        }

        [Fact]
        public void BeginWithLateBound_Behavior_ShouldAddExecutable()
        {
            this.SetupCreateActionExecutableReturnsAnyExecutable();

            this.testee.Begin.With(() => A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(NumberOfExecutablesForBegin);
        }

        [Fact]
        public void BeginWithLateBound_BehaviorMultipleTimes_ShouldOnlyAddOneExecutable()
        {
            this.SetupCreateActionExecutableReturnsAnyExecutable();

            this.testee.Begin.With(() => A.Fake<IBehavior<ICustomExtension>>()).With(() => A.Fake<IBehavior<ICustomExtension>>()).With(() => A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(NumberOfExecutablesForBegin);
        }

        [Fact]
        public void BeginWithLateBound_Behavior_ShouldAddBehaviorToLastExecutable()
        {
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var behavior = A.Fake<IBehavior<ICustomExtension>>();

            this.SetupCreateActionExecutableReturnsExecutable(extension);

            this.testee.Begin.With(() => behavior);

            A.CallTo(() => extension.Add(A<IBehavior<ICustomExtension>>._)).MustHaveHappened();
        }

        [Fact]
        public void BeginWithLateBound_BehaviorMultipleTimes_ShouldAddBehaviorToLastExecutable()
        {
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var firstBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var secondBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var thirdBehavior = A.Fake<IBehavior<ICustomExtension>>();

            this.SetupCreateActionExecutableReturnsExecutable(extension);

            this.testee
                .Begin
                    .With(() => firstBehavior)
                    .With(() => secondBehavior)
                    .With(() => thirdBehavior);

            A.CallTo(() => extension.Add(A<IBehavior<ICustomExtension>>._)).MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public void BeginWithLateBound_Behavior_ShouldBehaveLateBound()
        {
            IBehavior<ICustomExtension> interceptedBehavior = null;
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var behavior = A.Fake<IBehavior<ICustomExtension>>();

            this.SetupCreateActionExecutableReturnsExecutable(extension);
            A.CallTo(() => extension.Add(A<IBehavior<ICustomExtension>>._)).Invokes((IBehavior<ICustomExtension> b) => interceptedBehavior = b);

            this.testee.Begin.With(() => behavior);

            interceptedBehavior.Behave(Enumerable.Empty<ICustomExtension>());

            A.CallTo(() => behavior.Behave(Enumerable.Empty<ICustomExtension>())).MustHaveHappened();
        }

        [Fact]
        public void EndWith_Behavior_ShouldAddExecutable()
        {
            this.SetupCreateActionExecutableReturnsAnyExecutable();

            this.testee.Begin.End.With(A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(NumberOfExecutablesForEnd);
        }

        [Fact]
        public void EndWith_BehaviorMultipleTimes_ShouldOnlyAddOneExecutable()
        {
            this.SetupCreateActionExecutableReturnsAnyExecutable();

            this.testee.Begin.End.With(A.Fake<IBehavior<ICustomExtension>>()).With(A.Fake<IBehavior<ICustomExtension>>()).With(A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(NumberOfExecutablesForEnd);
        }

        [Fact]
        public void EndWith_Behavior_ShouldAddBehaviorToLastExecutable()
        {
            var firstExtension = A.Fake<IExecutable<ICustomExtension>>();
            var secondExtension = A.Fake<IExecutable<ICustomExtension>>();
            var behavior = A.Fake<IBehavior<ICustomExtension>>();

            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action>>._))
                .ReturnsNextFromSequence(firstExtension, secondExtension);

            this.testee.Begin.End.With(behavior);

            A.CallTo(() => firstExtension.Add(behavior)).MustNotHaveHappened();
            A.CallTo(() => secondExtension.Add(behavior)).MustHaveHappened();
        }

        [Fact]
        public void EndWith_BehaviorMultipleTimes_ShouldAddBehaviorToLastExecutable()
        {
            var firstExtension = A.Fake<IExecutable<ICustomExtension>>();
            var secondExtension = A.Fake<IExecutable<ICustomExtension>>();
            var firstBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var secondBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var thirdBehavior = A.Fake<IBehavior<ICustomExtension>>();

            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action>>._))
                .ReturnsNextFromSequence(firstExtension, secondExtension);

            this.testee
                .Begin.End
                    .With(firstBehavior)
                    .With(secondBehavior)
                    .With(thirdBehavior);

            A.CallTo(() => firstExtension.Add(firstBehavior)).MustNotHaveHappened();
            A.CallTo(() => firstExtension.Add(secondBehavior)).MustNotHaveHappened();
            A.CallTo(() => firstExtension.Add(thirdBehavior)).MustNotHaveHappened();
            
            A.CallTo(() => secondExtension.Add(firstBehavior)).MustHaveHappened();
            A.CallTo(() => secondExtension.Add(secondBehavior)).MustHaveHappened();
            A.CallTo(() => secondExtension.Add(thirdBehavior)).MustHaveHappened();
        }

        [Fact]
        public void EndWithLateBound_Behavior_ShouldAddExecutable()
        {
            this.SetupCreateActionExecutableReturnsAnyExecutable();

            this.testee.Begin.End.With(() => A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(NumberOfExecutablesForEnd);
        }

        [Fact]
        public void EndWithLateBound_BehaviorMultipleTimes_ShouldOnlyAddOneExecutable()
        {
            this.SetupCreateActionExecutableReturnsAnyExecutable();

            this.testee.Begin.End.With(() => A.Fake<IBehavior<ICustomExtension>>()).With(() => A.Fake<IBehavior<ICustomExtension>>()).With(() => A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(NumberOfExecutablesForEnd);
        }

        [Fact]
        public void EndWithLateBound_Behavior_ShouldAddBehaviorToLastExecutable()
        {
            var firstExtension = A.Fake<IExecutable<ICustomExtension>>();
            var secondExtension = A.Fake<IExecutable<ICustomExtension>>();
            var behavior = A.Fake<IBehavior<ICustomExtension>>();

            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action>>._))
                .ReturnsNextFromSequence(firstExtension, secondExtension);

            this.testee.Begin.End.With(() => behavior);

            A.CallTo(() => firstExtension.Add(A<IBehavior<ICustomExtension>>._)).MustNotHaveHappened();
            A.CallTo(() => secondExtension.Add(A<IBehavior<ICustomExtension>>._)).MustHaveHappened();
        }

        [Fact]
        public void EndWithLateBound_BehaviorMultipleTimes_ShouldAddBehaviorToLastExecutable()
        {
            var firstExtension = A.Fake<IExecutable<ICustomExtension>>();
            var secondExtension = A.Fake<IExecutable<ICustomExtension>>();
            var firstBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var secondBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var thirdBehavior = A.Fake<IBehavior<ICustomExtension>>();

            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action>>._))
                .ReturnsNextFromSequence(firstExtension, secondExtension);

            this.testee
                .Begin.End
                    .With(() => firstBehavior)
                    .With(() => secondBehavior)
                    .With(() => thirdBehavior);

            A.CallTo(() => firstExtension.Add(A<IBehavior<ICustomExtension>>._)).MustNotHaveHappened();
            A.CallTo(() => secondExtension.Add(A<IBehavior<ICustomExtension>>._)).MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public void EndWithLateBound_Behavior_ShouldBehaveLateBound()
        {
            IBehavior<ICustomExtension> interceptedBehavior = null;
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var behavior = A.Fake<IBehavior<ICustomExtension>>();

            this.SetupCreateActionExecutableReturnsExecutable(extension);
            A.CallTo(() => extension.Add(A<IBehavior<ICustomExtension>>._)).Invokes((IBehavior<ICustomExtension> b) => interceptedBehavior = b);

            this.testee.Begin.End.With(() => behavior);

            interceptedBehavior.Behave(Enumerable.Empty<ICustomExtension>());

            A.CallTo(() => behavior.Behave(Enumerable.Empty<ICustomExtension>())).MustHaveHappened();
        }

        [Fact]
        public void WithAfterExecuteWithAction_Behavior_ShouldAddExecutable()
        {
            this.SetupCreateActionExecutableReturnsAnyExecutable();

            this.testee.Execute(() => this.DoNothing()).With(A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(1);
        }

        [Fact]
        public void WithAfterExecuteWithAction_BehaviorMultipleTimes_ShouldOnlyAddOneExecutable()
        {
            this.SetupCreateActionExecutableReturnsAnyExecutable();

            this.testee.Execute(() => this.DoNothing()).With(A.Fake<IBehavior<ICustomExtension>>()).With(A.Fake<IBehavior<ICustomExtension>>()).With(A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(1);
        }

        [Fact]
        public void WithAfterExecuteWithAction_Behavior_ShouldAddBehaviorToLastExecutable()
        {
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var behavior = A.Fake<IBehavior<ICustomExtension>>();

            this.SetupCreateActionExecutableReturnsExecutable(extension);

            this.testee
                .Execute(() => this.DoNothing())
                    .With(behavior);

            A.CallTo(() => extension.Add(behavior)).MustHaveHappened();
        }

        [Fact]
        public void WithAfterExecuteWithAction_BehaviorMultipleTimes_ShouldAddBehaviorToLastExecutable()
        {
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var firstBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var secondBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var thirdBehavior = A.Fake<IBehavior<ICustomExtension>>();

            this.SetupCreateActionExecutableReturnsExecutable(extension);

            this.testee
                .Execute(() => this.DoNothing())
                    .With(firstBehavior)
                    .With(secondBehavior)
                    .With(thirdBehavior);

            A.CallTo(() => extension.Add(firstBehavior)).MustHaveHappened();
            A.CallTo(() => extension.Add(secondBehavior)).MustHaveHappened();
            A.CallTo(() => extension.Add(thirdBehavior)).MustHaveHappened();
        }

        [Fact]
        public void WithLateBoundAfterExecuteWithAction_Behavior_ShouldAddExecutable()
        {
            this.SetupCreateActionExecutableReturnsAnyExecutable();

            this.testee.Execute(() => this.DoNothing()).With(() => A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(1);
        }

        [Fact]
        public void WithLateBoundAfterExecuteWithAction_BehaviorMultipleTimes_ShouldOnlyAddOneExecutable()
        {
            this.SetupCreateActionExecutableReturnsAnyExecutable();

            this.testee.Execute(() => this.DoNothing()).With(() => A.Fake<IBehavior<ICustomExtension>>()).With(() => A.Fake<IBehavior<ICustomExtension>>()).With(() => A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(1);
        }

        [Fact]
        public void WithLateBoundAfterExecuteWithAction_Behavior_ShouldAddBehaviorToLastExecutable()
        {
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var behavior = A.Fake<IBehavior<ICustomExtension>>();

            this.SetupCreateActionExecutableReturnsExecutable(extension);

            this.testee
                .Execute(() => this.DoNothing())
                    .With(() => behavior);

            A.CallTo(() => extension.Add(A<IBehavior<ICustomExtension>>._)).MustHaveHappened();
        }

        [Fact]
        public void WithLateBoundAfterExecuteWithAction_BehaviorMultipleTimes_ShouldAddBehaviorToLastExecutable()
        {
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var firstBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var secondBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var thirdBehavior = A.Fake<IBehavior<ICustomExtension>>();

            this.SetupCreateActionExecutableReturnsExecutable(extension);

            this.testee
                .Execute(() => this.DoNothing())
                    .With(() => firstBehavior)
                    .With(() => secondBehavior)
                    .With(() => thirdBehavior);

            A.CallTo(() => extension.Add(A<IBehavior<ICustomExtension>>._)).MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public void WithLateBoundAfterExecuteWithAction_Behavior_ShouldBehaveLateBound()
        {
            IBehavior<ICustomExtension> interceptedBehavior = null;
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var behavior = A.Fake<IBehavior<ICustomExtension>>();

            this.SetupCreateActionExecutableReturnsExecutable(extension);
            A.CallTo(() => extension.Add(A<IBehavior<ICustomExtension>>._)).Invokes((IBehavior<ICustomExtension> b) => interceptedBehavior = b);

            this.testee
                .Execute(() => this.DoNothing())
                    .With(() => behavior);

            interceptedBehavior.Behave(Enumerable.Empty<ICustomExtension>());

            A.CallTo(() => behavior.Behave(Enumerable.Empty<ICustomExtension>())).MustHaveHappened();
        }

        [Fact]
        public void WithAfterExecuteWithActionOnExtension_Behavior_ShouldAddExecutable()
        {
            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action<ICustomExtension>>>._)).Returns(A.Fake<IExecutable<ICustomExtension>>());

            this.testee
                .Execute(e => e.Dispose())
                    .With(A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(1);
        }

        [Fact]
        public void WithAfterExecuteWithActionOnExtension_BehaviorMultipleTimes_ShouldOnlyAddOneExecutable()
        {
            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action<ICustomExtension>>>._)).Returns(A.Fake<IExecutable<ICustomExtension>>());

            this.testee.Execute(e => e.Dispose())
                .With(A.Fake<IBehavior<ICustomExtension>>())
                .With(A.Fake<IBehavior<ICustomExtension>>())
                .With(A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(1);
        }

        [Fact]
        public void WithAfterExecuteWithActionOnExtension_Behavior_ShouldAddBehaviorToLastExecutable()
        {
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var behavior = A.Fake<IBehavior<ICustomExtension>>();

            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action<ICustomExtension>>>._)).Returns(extension);

            this.testee
                .Execute(e => e.Dispose())
                    .With(behavior);

            A.CallTo(() => extension.Add(behavior)).MustHaveHappened();
        }

        [Fact]
        public void WithAfterExecuteWithActionOnExtension_BehaviorMultipleTimes_ShouldAddBehaviorToLastExecutable()
        {
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var firstBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var secondBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var thirdBehavior = A.Fake<IBehavior<ICustomExtension>>();

            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action<ICustomExtension>>>._)).Returns(extension);

            this.testee
                .Execute(e => e.Dispose())
                    .With(firstBehavior)
                    .With(secondBehavior)
                    .With(thirdBehavior);

            A.CallTo(() => extension.Add(firstBehavior)).MustHaveHappened();
            A.CallTo(() => extension.Add(secondBehavior)).MustHaveHappened();
            A.CallTo(() => extension.Add(thirdBehavior)).MustHaveHappened();
        }

        [Fact]
        public void WithBoundLateAfterExecuteWithActionOnExtension_Behavior_ShouldAddExecutable()
        {
            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action<ICustomExtension>>>._)).Returns(A.Fake<IExecutable<ICustomExtension>>());

            this.testee
                .Execute(e => e.Dispose())
                    .With(() => A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(1);
        }

        [Fact]
        public void WithLateBoundAfterExecuteWithActionOnExtension_BehaviorMultipleTimes_ShouldOnlyAddOneExecutable()
        {
            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action<ICustomExtension>>>._)).Returns(A.Fake<IExecutable<ICustomExtension>>());

            this.testee.Execute(e => e.Dispose())
                .With(() => A.Fake<IBehavior<ICustomExtension>>())
                .With(() => A.Fake<IBehavior<ICustomExtension>>())
                .With(() => A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(1);
        }

        [Fact]
        public void WithLateBoundAfterExecuteWithActionOnExtension_Behavior_ShouldAddBehaviorToLastExecutable()
        {
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var behavior = A.Fake<IBehavior<ICustomExtension>>();

            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action<ICustomExtension>>>._)).Returns(extension);

            this.testee
                .Execute(e => e.Dispose())
                    .With(() => behavior);

            A.CallTo(() => extension.Add(A<IBehavior<ICustomExtension>>._)).MustHaveHappened();
        }

        [Fact]
        public void WithLateBoundAfterExecuteWithActionOnExtension_BehaviorMultipleTimes_ShouldAddBehaviorToLastExecutable()
        {
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var firstBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var secondBehavior = A.Fake<IBehavior<ICustomExtension>>();
            var thirdBehavior = A.Fake<IBehavior<ICustomExtension>>();

            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action<ICustomExtension>>>._)).Returns(extension);

            this.testee
                .Execute(e => e.Dispose())
                    .With(() => firstBehavior)
                    .With(() => secondBehavior)
                    .With(() => thirdBehavior);

            A.CallTo(() => extension.Add(A<IBehavior<ICustomExtension>>._)).MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public void WithLateBoundAfterExecuteWithActionOnExtension_Behavior_ShouldBehaveLateBound()
        {
            IBehavior<ICustomExtension> interceptedBehavior = null;
            var extension = A.Fake<IExecutable<ICustomExtension>>();
            var behavior = A.Fake<IBehavior<ICustomExtension>>();

            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action<ICustomExtension>>>._)).Returns(extension);
            A.CallTo(() => extension.Add(A<IBehavior<ICustomExtension>>._)).Invokes((IBehavior<ICustomExtension> b) => interceptedBehavior = b);

            this.testee
                .Execute(e => e.Dispose())
                    .With(() => behavior);

            interceptedBehavior.Behave(Enumerable.Empty<ICustomExtension>());

            A.CallTo(() => behavior.Behave(Enumerable.Empty<ICustomExtension>())).MustHaveHappened();
        }

        [Fact]
        public void WithAfterExecuteWithActionOnExtensionWithInitializer_Behavior_ShouldAddExecutable()
        {
            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Func<object>>>._, A<Expression<Action<ICustomExtension, object>>>._, A<Action<IBehaviorAware<ICustomExtension>, object>>._)).Returns(A.Fake<IExecutable<ICustomExtension>>());

            this.testee
                .Execute(() => new object(), (e, o) => e.Dispose())
                    .With(o => A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(1);
        }

        [Fact]
        public void WithAfterExecuteWithActionOnExtensionWithInitializer_BehaviorMultipleTimes_ShouldOnlyAddOneExecutable()
        {
            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Func<object>>>._, A<Expression<Action<ICustomExtension, object>>>._, A<Action<IBehaviorAware<ICustomExtension>, object>>._)).Returns(A.Fake<IExecutable<ICustomExtension>>());

            this.testee.Execute(() => new object(), (e, o) => e.Dispose())
                .With(o => A.Fake<IBehavior<ICustomExtension>>())
                .With(o => A.Fake<IBehavior<ICustomExtension>>())
                .With(o => A.Fake<IBehavior<ICustomExtension>>());

            this.testee.Should().HaveCount(1);
        }

        [Fact]
        public void WithAfterExecuteWithActionOnExtensionWithInitializer_Behavior_ShouldAddBehaviorToLastExecutable()
        {
            IBehavior<ICustomExtension> behavior = null;
            Action<IBehaviorAware<ICustomExtension>, object> contextInitializer = null;

            var extension = A.Fake<IExecutable<ICustomExtension>>();
            A.CallTo(() => extension.Add(A<IBehavior<ICustomExtension>>._)).Invokes((IBehavior<ICustomExtension> b) => behavior = b);

            A.CallTo(() => this.executableFactory.CreateExecutable(
                A<Expression<Func<object>>>._, 
                A<Expression<Action<ICustomExtension, object>>>._, 
                A<Action<IBehaviorAware<ICustomExtension>, object>>._))
                .Invokes((Expression<Func<object>> func, Expression<Action<ICustomExtension, object>> action, Action<IBehaviorAware<ICustomExtension>, object> ctx) => contextInitializer = ctx)
                .Returns(A.Fake<IExecutable<ICustomExtension>>());

            var context = new object();

            this.testee.Execute(() => context, (e, o) => e.Dispose()).With(o => new TestableBehavior(o));

            contextInitializer(extension, context);

            behavior.Should().NotBeNull();
            behavior.As<TestableBehavior>().Context.Should().Be(context);
        }

        private class TestableBehavior : IBehavior<ICustomExtension>
        {
            private readonly object context;

            public TestableBehavior(object context)
            {
                this.context = context;
            }

            /// <inheritdoc />
            public string Name
            {
                get
                {
                    return this.GetType().FullNameToString();
                }
            }

            public object Context
            {
                get
                {
                    return this.context;
                }
            }

            public void Behave(IEnumerable<ICustomExtension> extensions)
            {
            }

            public string Describe()
            {
                return "Behaves by doing nothing.";
            }
        }

        [Theory,
         InlineData("ABC", "ABCI"),
         InlineData("CBA", "CIBA"),
         InlineData("AAA", "AAA"),
         InlineData("BBB", "BBB"),
         InlineData("CCC", "CICICI")]
        public void Execute_Chaining_ShouldBePossible(string execution, string expected)
        {
            this.ExecuteChaining(execution);

            this.executionChainingBuilder.ToString().Should().Be(expected);
        }

        [Theory,
        InlineData("ABC", 3),
         InlineData("CBA", 3),
         InlineData("AAA", 3),
         InlineData("BBB", 3),
         InlineData("CCC", 3),
         InlineData("AAAA", 4),
         InlineData("AAAAA", 5)]
        public void Enumeration_ShouldProvideDefinedExecutables(string execution, int expected)
        {
            this.ExecuteChaining(execution);

            this.testee.Count().Should().Be(expected);
        }

        private void DoNothing()
        {
        }

        private void ExecuteChaining(string syntax)
        {
            this.SetupAutoExecutionOfExecutables();

            Dictionary<char, Action> actions = this.DefineCharToActionMapping();

            foreach (char c in syntax.ToUpperInvariant())
            {
                actions[c].Invoke();
            }
        }

        private void SetupAutoExecutionOfExecutables()
        {
            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action>>._))
                .Invokes((Expression<Action> action) => action.Compile()())
                .Returns(A.Fake<IExecutable<ICustomExtension>>());
            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action<ICustomExtension>>>._))
                .Invokes((Expression<Action<ICustomExtension>> action) => action.Compile()(A.Fake<ICustomExtension>()))
                .Returns(A.Fake<IExecutable<ICustomExtension>>());
            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Func<char>>>._, A<Expression<Action<ICustomExtension, char>>>._, A<Action<IBehaviorAware<ICustomExtension>, char>>._))
                .Invokes((Expression<Func<char>> func, Expression<Action<ICustomExtension, char>> action, Action<IBehaviorAware<ICustomExtension>, char> context) =>
                    {
                        var ctx = func.Compile()();
                        context(A.Fake<IBehaviorAware<ICustomExtension>>(), ctx);
                        action.Compile()(A.Fake<ICustomExtension>(), ctx);
                    })
                .Returns(A.Fake<IExecutable<ICustomExtension>>());
        }

        private Dictionary<char, Action> DefineCharToActionMapping()
        {
            return new Dictionary<char, Action>
                {
                    { 'A', () => this.testee.Execute(() => this.executionChainingBuilder.Append('A')) },
                    { 'B', () => this.testee.Execute(extension => this.executionChainingBuilder.Append('B')) },
                    {
                        'C', () => this.testee.Execute(
                            () => 'I',
                            (extension, context) => this.AppendValue(context))
                        },
                };
        }

        private void AppendValue(char context)
        {
            this.executionChainingBuilder.Append('C');
            this.executionChainingBuilder.Append(context);
        }

        private void SetupCreateActionExecutableReturnsExecutable(IExecutable<ICustomExtension> executable)
        {
            A.CallTo(() => this.executableFactory.CreateExecutable(A<Expression<Action>>._)).Returns(executable);
        }

        private void SetupCreateActionExecutableReturnsAnyExecutable()
        {
            this.SetupCreateActionExecutableReturnsExecutable(A.Fake<IExecutable<ICustomExtension>>());
        }
    }
}