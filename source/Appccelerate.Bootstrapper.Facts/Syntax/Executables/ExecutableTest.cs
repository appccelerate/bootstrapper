//-------------------------------------------------------------------------------
// <copyright file="ExecutableTest.cs" company="Appccelerate">
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

namespace Appccelerate.Bootstrapper.Syntax.Executables
{
    using System.Collections.Generic;
    using System.Linq;
    using Appccelerate.Bootstrapper.Dummies;
    using Appccelerate.Bootstrapper.Reporting;
    using Appccelerate.Formatters;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit.Extensions;

    public class ExecutableTest
    {
        private readonly IExecutableContext executableContext;

        public ExecutableTest()
        {
            this.executableContext = A.Fake<IExecutableContext>();
        }

        public static IEnumerable<object[]> Testees
        {
            get
            {
                yield return new object[] { new ActionExecutable<ICustomExtension>(() => ActionHelper()) };
                yield return new object[] { new ActionOnExtensionExecutable<ICustomExtension>(x => x.Dispose()) };
                yield return
                    new object[]
                        {
                            new ActionOnExtensionWithInitializerExecutable<object, ICustomExtension>(
                                () => FunctionHelper(), (x, i) => x.SomeMethod(i), (aware, ctx) => { })
                        };
            }
        }

        [Theory]
        [PropertyData("Testees")]
        public void ShouldReturnTypeName(IExecutable<ICustomExtension> testee)
        {
            string expectedName = testee.GetType().FullNameToString();

            testee.Name.Should().Be(expectedName);
        }

        [Theory]
        [PropertyData("Testees")]
        public void Execute_ShouldExecuteBehavior(IExecutable<ICustomExtension> testee)
        {
            var first = A.Fake<IBehavior<ICustomExtension>>();
            var second = A.Fake<IBehavior<ICustomExtension>>();
            var extensions = Enumerable.Empty<ICustomExtension>().ToList();

            testee.Add(first);
            testee.Add(second);

            testee.Execute(extensions, this.executableContext);

            A.CallTo(() => first.Behave(extensions)).MustHaveHappened();
            A.CallTo(() => second.Behave(extensions)).MustHaveHappened();
        }

        [Theory]
        [PropertyData("Testees")]
        public void Execute_ShouldCreateBehaviorContextForBehaviors(IExecutable<ICustomExtension> testee)
        {
            var first = A.Fake<IBehavior<ICustomExtension>>();
            var second = A.Fake<IBehavior<ICustomExtension>>();

            testee.Add(first);
            testee.Add(second);

            testee.Execute(Enumerable.Empty<ICustomExtension>(), this.executableContext);

            A.CallTo(() => this.executableContext.CreateBehaviorContext(first));
            A.CallTo(() => this.executableContext.CreateBehaviorContext(second));
        }

        private static void ActionHelper()
        {
        }

        private static object FunctionHelper()
        {
            return null;
        }
    }
}