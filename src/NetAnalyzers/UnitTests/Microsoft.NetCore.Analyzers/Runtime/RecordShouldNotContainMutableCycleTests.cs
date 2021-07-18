// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Xunit;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace Microsoft.NetCore.Analyzers.Runtime.UnitTests
{
    public class RecordShouldNotContainMutableCycleTests
    {
        [Fact]
        public async Task NoCycle_Diagnostic()
        {
            var source = @"
using System;

record Rec
{
    public string Field;
}
";
            await TestAsync(source);
        }

        [Fact]
        public async Task DirectMutableCycle_Diagnostic()
        {
            var source = @"
using System;

record Rec
{
    public Rec Inner;
}
";
            await TestAsync(source);
        }

        private Task TestAsync(string source, params DiagnosticResult[] expected)
        {
            var test = new CSharpAnalyzerTest<RecordShouldNotContainMutableCycleAnalyzer, XUnitVerifier>
            {
                TestCode = source,
                SolutionTransforms =
                {
                    (solution, projectId) =>
                    {
                        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp9);
                        return solution.WithProjectParseOptions(projectId, parseOptions);
                    }
                }
            };
            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync();
        }
    }
}