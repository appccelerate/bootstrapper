//-------------------------------------------------------------------------------
// <copyright file="SyntaxBuilderWithContextTest.cs" company="Appccelerate">
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

namespace Appccelerate.Bootstrapper.Syntax
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Appccelerate.Bootstrapper.Dummies;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class SyntaxBuilderWithContextTest
    {
        private readonly ISyntaxBuilderWithoutContext<ICustomExtension> syntaxBuilder;
        private readonly IEndWithBehavior<ICustomExtension> endWithBehavior;
        private readonly Queue<Func<object, IBehavior<ICustomExtension>>> behaviorProviders;

        private readonly SyntaxBuilderWithContext<ICustomExtension, object> testee;

        public SyntaxBuilderWithContextTest()
        {
            this.behaviorProviders = new Queue<Func<object, IBehavior<ICustomExtension>>>();

            this.syntaxBuilder = A.Fake<ISyntaxBuilderWithoutContext<ICustomExtension>>();
            this.endWithBehavior = this.syntaxBuilder.As<IEndWithBehavior<ICustomExtension>>();

            this.testee = new SyntaxBuilderWithContext<ICustomExtension, object>(this.syntaxBuilder, this.behaviorProviders);
        }

        [Fact]
        public void End_ShouldDelegateToInternal()
        {
            IEndWithBehavior<ICustomExtension> result = this.testee.End;

            A.CallTo(() => this.syntaxBuilder.End).MustHaveHappened();
        }

        [Fact]
        public void Execute_WithAction_ShouldDelegateToInternal()
        {
            Action action = () => { };
            Expression<Action> expression = () => action();

            this.testee.Execute(expression);

            A.CallTo(() => this.syntaxBuilder.Execute(expression)).MustHaveHappened();
        }

        [Fact]
        public void Execute_WithActionOnExtension_ShouldDelegateToInternal()
        {
            Action action = () => { };
            Expression<Action> expression = () => action();

            this.testee.Execute(expression);

            A.CallTo(() => this.syntaxBuilder.Execute(expression)).MustHaveHappened();
        }

        [Fact]
        public void Execute_WithInitializerAndActionOnExtension_ShouldDelegateToInternal()
        {
            Func<object> initializer = () => new object();
            Action<ICustomExtension, object> action = (e, ctx) => { };

            Expression<Func<object>> initializationExpression = () => initializer();
            Expression<Action<ICustomExtension, object>> expression = (e, ctx) => action(e, ctx);

            this.testee.Execute(initializationExpression, expression);

            A.CallTo(() => this.syntaxBuilder.Execute(initializationExpression, expression)).MustHaveHappened();
        }

        [Fact]
        public void With_WithConstantBehaviorWhichReturnsEndWithBehavior_ShouldDelegateToInternal()
        {
            var behavior = A.Fake<IBehavior<ICustomExtension>>();

            this.testee.With(behavior);

            A.CallTo(() => this.endWithBehavior.With(behavior)).MustHaveHappened();
        }

        [Fact]
        public void With_WithLazyBehaviorWhichReturnsEndWithBehavior_ShouldDelegateToInternal()
        {
            Expression<Func<IBehavior<ICustomExtension>>> behaviorProvider = () => A.Fake<IBehavior<ICustomExtension>>();

            this.testee.With(behaviorProvider);

            A.CallTo(() => this.endWithBehavior.With(behaviorProvider)).MustHaveHappened();
        }

        [Fact]
        public void With_WithBehaviorOnContext_ShouldTrackBehaviorProviders()
        {
            var behavior = A.Fake<IBehavior<ICustomExtension>>();
            var anyObject = new object();

            this.testee.With(ctx => behavior);

            this.behaviorProviders.Should().HaveCount(1);
            this.behaviorProviders.Single()(anyObject).Should().Be(behavior);
        }

        [Fact]
        public void GetEnumerator_ShouldDelegateToInternal()
        {
            var enumerator = this.testee.GetEnumerator();

            A.CallTo(() => this.syntaxBuilder.GetEnumerator()).MustHaveHappened();
        }
    }
}