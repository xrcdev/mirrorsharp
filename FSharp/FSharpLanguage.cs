﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.FSharp.Compiler.AbstractIL.Internal;
using Microsoft.FSharp.Core;
using MirrorSharp.Internal.Abstraction;

namespace MirrorSharp.FSharp {
    public class FSharpLanguage : ILanguage {
        public const string Name = "F#";

        ParseOptions ILanguage.DefaultParseOptions => null;
        CompilationOptions ILanguage.DefaultCompilationOptions => null;
        private readonly ImmutableArray<string> _defaultAssemblyReferencePaths;

        static FSharpLanguage() {
            Library.Shim.FileSystem = new RestrictedFileSystem();
        }

        internal FSharpLanguage() {
            var fsharpAssembly = typeof(FSharpOption<>).GetTypeInfo().Assembly;
            _defaultAssemblyReferencePaths = ImmutableArray.Create<string>(
                // note: this currently does not work on .NET Core, which is why this project isn't netstandard
                typeof(object).GetTypeInfo().Assembly.Location,
                new Uri(fsharpAssembly.EscapedCodeBase).LocalPath
            );
        }

        ILanguageSession ILanguage.CreateSession(string text, ParseOptions parseOptions, CompilationOptions compilationOptions, IReadOnlyCollection<MetadataReference> assemblyReferences) {
            if (assemblyReferences != null)
                throw new NotSupportedException();

            return new FSharpSession(text, _defaultAssemblyReferencePaths);
        }

        string ILanguage.Name => Name;
    }
}