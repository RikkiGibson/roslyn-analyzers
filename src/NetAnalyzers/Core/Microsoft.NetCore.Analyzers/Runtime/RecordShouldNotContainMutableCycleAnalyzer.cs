// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Analyzer.Utilities;
using Analyzer.Utilities.Extensions;
using Analyzer.Utilities.PooledObjects;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Microsoft.NetCore.Analyzers.Runtime
{
    /// <summary>
    /// CS6001: Record should not contain mutable cycle
    /// When a record with synthesized ToString/GetHashCode/Equals
    /// contains a mutable field which leads back to the original record,
    /// it's possible for a stack overflow to occur via compiler-generated code.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class RecordShouldNotContainMutableCycleAnalyzer : DiagnosticAnalyzer
    {
        internal const string RuleId = "CA6001";

        private static readonly LocalizableString s_localizableTitle = new LocalizableResourceString(nameof(MicrosoftNetCoreAnalyzersResources.RecordShouldNotContainMutableCycleTitle), MicrosoftNetCoreAnalyzersResources.ResourceManager, typeof(MicrosoftNetCoreAnalyzersResources));

        // TODO: messages
        private static readonly LocalizableString s_localizableMessageDefault = new LocalizableResourceString(nameof(MicrosoftNetCoreAnalyzersResources.AttributeStringLiteralsShouldParseCorrectlyMessageDefault), MicrosoftNetCoreAnalyzersResources.ResourceManager, typeof(MicrosoftNetCoreAnalyzersResources));
        private static readonly LocalizableString s_localizableMessageEmpty = new LocalizableResourceString(nameof(MicrosoftNetCoreAnalyzersResources.AttributeStringLiteralsShouldParseCorrectlyMessageEmpty), MicrosoftNetCoreAnalyzersResources.ResourceManager, typeof(MicrosoftNetCoreAnalyzersResources));
        private static readonly LocalizableString s_localizableDescription = new LocalizableResourceString(nameof(MicrosoftNetCoreAnalyzersResources.AttributeStringLiteralsShouldParseCorrectlyDescription), MicrosoftNetCoreAnalyzersResources.ResourceManager, typeof(MicrosoftNetCoreAnalyzersResources));

        internal static DiagnosticDescriptor DefaultRule = DiagnosticDescriptorHelper.Create(RuleId,
                                                                             s_localizableTitle,
                                                                             s_localizableMessageDefault,
                                                                             DiagnosticCategory.Usage,
                                                                             RuleLevel.Disabled,    // Heuristic based rule.
                                                                             description: s_localizableDescription,
                                                                             isPortedFxCopRule: true,
                                                                             isDataflowRule: false);
        internal static DiagnosticDescriptor EmptyRule = DiagnosticDescriptorHelper.Create(RuleId,
                                                                             s_localizableTitle,
                                                                             s_localizableMessageEmpty,
                                                                             DiagnosticCategory.Usage,
                                                                             RuleLevel.Disabled,    // Heuristic based rule.
                                                                             description: s_localizableDescription,
                                                                             isPortedFxCopRule: true,
                                                                             isDataflowRule: false);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DefaultRule, EmptyRule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            // in parallel we construct a map of each record to its immediate members (do we *really* need to do this?)
            // we need to bake in information about whether each field is mutable and which methods have compiler-generated implementations in each type.
            context.RegisterSymbolAction(visitRecord, SymbolKind.NamedType);

            void visitRecord(SymbolAnalysisContext context)
            {
                if (context.Symbol is not INamedTypeSymbol { IsRecord: true } record)
                {
                    return;
                }

                foreach (var member in record.GetMembers())
                {
                    if (member is IFieldSymbol { IsReadOnly: false, Type: { } type }
                        && type.Equals(record))
                    {
                        // TODO: this is obviously very basic. We are probably going to need something like
                        // a field graph so that we can look at each mutable field on this record
                        // and decide if mutation of the field could result in this instance being stored in the field graph.
                        context.ReportDiagnostic(member.CreateDiagnostic(DefaultRule));
                    }
                }
            }
        }
    }
}