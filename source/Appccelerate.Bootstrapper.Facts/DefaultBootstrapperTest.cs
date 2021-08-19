//-------------------------------------------------------------------------------
// <copyright file="DefaultBootstrapperTest.cs" company="Appccelerate">
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
    using System;
    using System.Collections.Generic;

    using Appccelerate.Bootstrapper.Reporting;
    using Appccelerate.Bootstrapper.Syntax;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class DefaultBootstrapperTest
    {
        private readonly IExtensionHost<IExtension> extensionHost;
        private readonly IReporter reporter;
        private readonly IExecutor<IExtension> runExecutor;
        private readonly IExecutor<IExtension> shutdownExecutor;
        private readonly IReportingContext reportingContext;
        private readonly IStrategy<IExtension> strategy;
        private readonly IExtensionResolver<IExtension> extensionResolver;

        private readonly DefaultBootstrapper<IExtension> testee;

        public DefaultBootstrapperTest()
        {
            this.extensionHost = A.Fake<IExtensionHost<IExtension>>();
            this.reporter = A.Fake<IReporter>();
            this.strategy = A.Fake<IStrategy<IExtension>>();
            this.runExecutor = A.Fake<IExecutor<IExtension>>();
            this.shutdownExecutor = A.Fake<IExecutor<IExtension>>();
            this.reportingContext = A.Fake<IReportingContext>();
            this.extensionResolver = A.Fake<IExtensionResolver<IExtension>>();

            this.testee = new DefaultBootstrapper<IExtension>(this.extensionHost, this.reporter);
        }

        [Fact]
        public void Initialize_MultipleTimes_ShouldThrowException()
        {
            this.SetupStrategyReturnsBuilderAndContextAndResolver();

            this.testee.Initialize(this.strategy);

            this.testee.Invoking(x => x.Initialize(A.Fake<IStrategy<IExtension>>())).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Initialize_ShouldCreateReportingContext()
        {
            this.SetupStrategyReturnsBuilderAndContextAndResolver();

            this.testee.Initialize(this.strategy);

            A.CallTo(() => this.strategy.CreateReportingContext()).MustHaveHappened();
        }

        [Fact]
        public void AddExtension_WhenNotInitialized_ShouldThrowInvalidOperationException()
        {
            this.testee.Invoking(x => x.AddExtension(A.Fake<IExtension>())).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void AddExtension_ShouldTrackExtension()
        {
            this.InitializeTestee();

            var extension = A.Fake<IExtension>();

            this.testee.AddExtension(extension);

            A.CallTo(() => this.extensionHost.AddExtension(extension)).MustHaveHappened();
        }

        [Fact]
        public void AddExtension_ShouldCreateExtensionContext()
        {
            this.InitializeTestee();

            var extension = A.Fake<IExtension>();

            this.testee.AddExtension(extension);

            A.CallTo(() => this.reportingContext.CreateExtensionContext(extension)).MustHaveHappened();
        }

        [Fact]
        public void Run_ShouldBuildRunSyntax()
        {
            this.InitializeTestee();

            this.testee.Run();

            A.CallTo(() => this.strategy.BuildRunSyntax()).MustHaveHappened();
        }

        [Fact]
        public void Run_ShouldCreateExtensionResolver()
        {
            this.InitializeTestee();

            this.testee.Run();

            A.CallTo(() => this.strategy.CreateExtensionResolver()).MustHaveHappened();
        }

        [Fact]
        public void Run_ShouldPassItselfToExtensionResolver()
        {
            this.InitializeTestee();

            this.testee.Run();

            A.CallTo(() => this.extensionResolver.Resolve(this.testee)).MustHaveHappened();
        }

        [Fact]
        public void Run_ShouldExecuteSyntaxAndExtensionsOnRunExecutor()
        {
            var runSyntax = A.Fake<ISyntax<IExtension>>();
            A.CallTo(() => this.strategy.BuildRunSyntax()).Returns(runSyntax);

            var extensions = new List<IExtension> { A.Fake<IExtension>(), };
            A.CallTo(() => this.extensionHost.Extensions).Returns(extensions);

            this.InitializeTestee();

            this.testee.Run();

            A.CallTo(() => this.runExecutor.Execute(runSyntax, extensions, A<IExecutionContext>._)).MustHaveHappened();
        }

        [Fact]
        public void Run_ShouldThrowExceptionWhenNotInitialized()
        {
            this.testee.Invoking(t => t.Run())
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Run_ShouldCreateRunExecutionContextWithRunExecutor()
        {
            this.InitializeTestee();

            this.testee.Run();

            A.CallTo(() => this.reportingContext.CreateRunExecutionContext(this.runExecutor)).MustHaveHappened();
        }

        [Fact]
        public void Run_ShouldProvideRunExecutionContextForRunExecutor()
        {
            var runExecutionContext = A.Fake<IExecutionContext>();

            A.CallTo(() => this.reportingContext.CreateRunExecutionContext(A<IDescribable>._)).Returns(runExecutionContext);

            this.InitializeTestee();

            this.testee.Run();

            A.CallTo(() => this.runExecutor.Execute(A<ISyntax<IExtension>>._, A<IEnumerable<IExtension>>._, runExecutionContext)).MustHaveHappened();
        }

        [Fact]
        public void Shutdown_ShouldBuildShutdownSyntax()
        {
            this.ShouldBuildShutdownSyntax(() => this.testee.Shutdown());
        }

        [Fact]
        public void Shutdown_ShouldExecuteSyntaxAndExtensionsOnShutdownExecutor()
        {
            this.ShouldExecuteSyntaxAndExtensionsOnShutdownExecutor(() => this.testee.Shutdown());
        }

        [Fact]
        public void Shutdown_ShouldThrowExceptionWhenNotInitialized()
        {
            this.testee.Invoking(t => t.Shutdown())
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Shutdown_ShouldCreateShutdownExecutionContextWithShutdownExecutor()
        {
            this.ShouldCreateShutdownExecutionContextWithShutdownExecutor(() => this.testee.Shutdown());
        }

        [Fact]
        public void Shutdown_ShouldProvideShutdownExecutionContextForShutdownExecutor()
        {
            this.ShouldProvideShutdownExecutionContextForShutdownExecutor(() => this.testee.Shutdown());
        }

        [Fact]
        public void Shutdown_ShouldReport()
        {
            this.ShouldReportBeforeStrategyDisposal(() => this.testee.Shutdown());
        }

        [Fact]
        public void Dispose_ShouldBuildShutdownSyntax()
        {
            this.ShouldBuildShutdownSyntax(() => this.testee.Dispose());
        }

        [Fact]
        public void Dispose_ShouldExecuteSyntaxAndExtensionsOnShutdownExecutor()
        {
            this.ShouldExecuteSyntaxAndExtensionsOnShutdownExecutor(() => this.testee.Dispose());
        }

        [Fact]
        public void Dispose_ShouldCreateShutdownExecutionContextWithShutdownExecutor()
        {
            this.ShouldCreateShutdownExecutionContextWithShutdownExecutor(() => this.testee.Dispose());
        }

        [Fact]
        public void Dispose_ShouldProvideShutdownExecutionContextForShutdownExecutor()
        {
            this.ShouldProvideShutdownExecutionContextForShutdownExecutor(() => this.testee.Dispose());
        }

        [Fact]
        public void Dispose_ShouldReport()
        {
            this.ShouldReportBeforeStrategyDisposal(() => this.testee.Dispose());
        }

        [Fact]
        public void Dispose_ShouldDisposeStrategy()
        {
            this.InitializeTestee();

            this.testee.Dispose();

            A.CallTo(() => this.strategy.Dispose()).MustHaveHappened();
        }

        private void ShouldCreateShutdownExecutionContextWithShutdownExecutor(Action executionAction)
        {
            this.InitializeTestee();

            executionAction();

            A.CallTo(() => this.reportingContext.CreateShutdownExecutionContext(this.shutdownExecutor)).MustHaveHappened();
        }

        private void ShouldProvideShutdownExecutionContextForShutdownExecutor(Action executionAction)
        {
            var shutdownExecutionContext = A.Fake<IExecutionContext>();

            A.CallTo(() => this.reportingContext.CreateShutdownExecutionContext(A<IDescribable>._)).Returns(shutdownExecutionContext);

            this.InitializeTestee();

            executionAction();

            A.CallTo(() => this.shutdownExecutor.Execute(A<ISyntax<IExtension>>._, A<IEnumerable<IExtension>>._, shutdownExecutionContext)).MustHaveHappened();
        }

        private void ShouldBuildShutdownSyntax(Action executionAction)
        {
            this.InitializeTestee();

            executionAction();

            A.CallTo(() => this.strategy.BuildShutdownSyntax()).MustHaveHappened();
        }

        private void ShouldExecuteSyntaxAndExtensionsOnShutdownExecutor(Action executionAction)
        {
            var shutdownSyntax = A.Fake<ISyntax<IExtension>>();
            A.CallTo(() => this.strategy.BuildShutdownSyntax()).Returns(shutdownSyntax);

            var extensions = new List<IExtension> { A.Fake<IExtension>(), };
            A.CallTo(() => this.extensionHost.Extensions).Returns(extensions);

            this.InitializeTestee();

            executionAction();

            A.CallTo(() => this.shutdownExecutor.Execute(shutdownSyntax, extensions, A<IExecutionContext>._));
        }

        private void ShouldReportBeforeStrategyDisposal(Action executionAction)
        {
            var queue = new Queue<string>();

            A.CallTo(() => this.strategy.Dispose()).Invokes(() => queue.Enqueue("Dispose"));
            A.CallTo(() => this.reporter.Report(A<IReportingContext>._)).Invokes((IReportingContext ctx) => queue.Enqueue("Report"));

            this.InitializeTestee();

            executionAction();

            queue.Should().ContainInOrder(new List<string> { "Report", "Dispose" });
        }

        private void SetupStrategyReturnsBuilderAndContextAndResolver()
        {
            A.CallTo(() => this.strategy.CreateReportingContext()).Returns(this.reportingContext);
            A.CallTo(() => this.strategy.CreateExtensionResolver()).Returns(this.extensionResolver);
            A.CallTo(() => this.strategy.CreateRunExecutor()).Returns(this.runExecutor);
            A.CallTo(() => this.strategy.CreateShutdownExecutor()).Returns(this.shutdownExecutor);
        }

        private void InitializeTestee()
        {
            this.SetupStrategyReturnsBuilderAndContextAndResolver();

            this.testee.Initialize(this.strategy);
        }
    }
}