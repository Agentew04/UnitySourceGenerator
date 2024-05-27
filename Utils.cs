using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using Microsoft.CodeAnalysis.Text;
using System.Linq;

namespace UnitySourceGenerator {
    internal static class Utils {
        // taken from https://github.com/LurkingNinja/DomainReloadSG

        internal static bool IsPartial(ClassDeclarationSyntax cds) =>
            cds.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));

        internal static bool IsAbstractClass(ClassDeclarationSyntax cds) =>
            cds.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.AbstractKeyword));

        internal static bool IsStatic(FieldDeclarationSyntax fds) =>
            fds.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.StaticKeyword));

        internal static bool IsReadOnly(FieldDeclarationSyntax fds) =>
            fds.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.ReadOnlyKeyword));

        internal static bool IsStatic(MethodDeclarationSyntax mds) =>
            mds.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.StaticKeyword));

        internal static bool IsVoidFunction(MethodDeclarationSyntax ms) =>
            ms.ReturnType is PredefinedTypeSyntax predefined && predefined.Keyword.IsKind(SyntaxKind.VoidKeyword);

        internal static bool HasAttribute(ClassDeclarationSyntax cds, string attributeName) =>
            cds.AttributeLists
                .Any(cdsAttributeList => cdsAttributeList.Attributes
                    .Any(cdsAttribute => cdsAttribute.ToString().Trim().ToLower()
                        .Equals(attributeName.Trim().ToLower())));

        internal static bool HasAttribute(FieldDeclarationSyntax fds, string attributeName) =>
            fds.AttributeLists
                .Any(fdsAttributeList => fdsAttributeList.Attributes
                                   .Any(fdsAttribute => fdsAttribute.ToString().Trim().ToLower()
                                                          .Equals(attributeName.Trim().ToLower())));

        internal static void AddSource(GeneratorExecutionContext context, string fileName, string source) =>
            context.AddSource($"{fileName}.g.cs", SourceText.From(source, Encoding.UTF8));

        //internal static void AddSource(GeneratorInitializationContext context, string fileName, string source) =>
        //    context.RegisterForPostInitialization(ctx =>
        //        ctx.AddSource($"{fileName}{FILENAME_POSTFIX}", SourceText.From(source, Encoding.UTF8)));

        internal static string GetNamespace(SyntaxNode node) {
            var nameSpace = string.Empty;
            var potentialNamespaceParent = node.Parent;

            while (potentialNamespaceParent != null
                   && !(potentialNamespaceParent is NamespaceDeclarationSyntax))
                potentialNamespaceParent = potentialNamespaceParent.Parent;

            if (!(potentialNamespaceParent is NamespaceDeclarationSyntax namespaceParent)) return nameSpace;

            nameSpace = namespaceParent.Name.ToString();

            while (true) {
                if (!(namespaceParent.Parent is NamespaceDeclarationSyntax parent)) break;

                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                namespaceParent = parent;
            }

            return string.IsNullOrEmpty(nameSpace)
                ? string.Empty
                : nameSpace;
        }

        internal static string GetUsingDirectives(ClassDeclarationSyntax cds) {
            var usingDirectives = new HashSet<string>();
            foreach (var child in cds.SyntaxTree.GetRoot().ChildNodes())
                if (child.IsKind(SyntaxKind.UsingDirective))
                    usingDirectives.Add(child.ToString());
            usingDirectives.Add("using System;");
            usingDirectives.Add("using UnityEngine;");

            return string.Join("\n", usingDirectives);
        }

        internal static void Log(string message) =>
            File.AppendAllText(@"D:\log.txt", $"{message}\n", Encoding.UTF8);
    }
}
