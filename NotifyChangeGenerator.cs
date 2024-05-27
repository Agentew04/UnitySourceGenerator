using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace UnitySourceGenerator {

    [Generator]
    public class NotifyChangeGenerator : ISourceGenerator {

        public void Initialize(GeneratorInitializationContext context) {
            context.RegisterForSyntaxNotifications(() => new SrcGenSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context) {
            
            if(!(context.SyntaxReceiver is SrcGenSyntaxReceiver receiver)) {
                return;
            }

            if(receiver.@class == null || receiver.fields.Count == 0) {
                return;
            }

            var namespaceName = Utils.GetNamespace(receiver.@class);
            var className = receiver.@class.Identifier.Text;

            string classCode;
            if(namespaceName != "") {
                classCode = string.Format(Templates.notifyChangeNamespacedClassCode,
                namespaceName,
                className,
                GenerateFields(receiver.fields));
            } else {
                classCode = string.Format(Templates.notifyChangeClassCode,
                    className,
                    GenerateFields(receiver.fields));
            }

            Utils.Log("Added source: " + classCode);

            Utils.AddSource(context, $"{className}.NotifyChange.g.cs", classCode);
            Utils.AddSource(context, "NotifyChangeAttribute.cs", Templates.NotifyChangeAttributeText);
        }

        private string GenerateFields(List<FieldDeclarationSyntax> fields) {
            StringBuilder sb = new StringBuilder();
            foreach(var field in fields) {
                /*
                 * 0 is fieldName
                 * 1 is typename
                 * 2 is accessibility modifier
                 * 3 is propertyName
                 */
                var type = field.Declaration.Type;
                var typename = type.ToString();
                var fieldName = field.Declaration.Variables.First().Identifier.Text;
                var propertyName = fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
                var fieldAccebilityModifier = field.Modifiers.First().ToString();
                sb.AppendFormat(Templates.notifyChangePartialMethodsCode, 
                    fieldName, typename, fieldAccebilityModifier, propertyName);
            }
            return sb.ToString();
        }
    }

    internal class SrcGenSyntaxReceiver : ISyntaxReceiver {

        internal ClassDeclarationSyntax @class { get; private set; }

        internal List<FieldDeclarationSyntax> fields { get; private set; } = new List<FieldDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if(!(syntaxNode is ClassDeclarationSyntax cds)) {
                return;
            }

            if(Utils.IsAbstractClass(cds) || !Utils.IsPartial(cds)) {
                return;
            }

            fields.Clear();
            foreach(var node in cds.ChildNodes()) {
                if(node is FieldDeclarationSyntax fds
                    && Utils.HasAttribute(fds, "NotifyChange")) {
                      fields.Add(fds);
                    Utils.Log("Added field " + fds.FullSpan.ToString());
                }
            }

            if(fields.Count > 0) {
                @class = cds;
            }
        }
    }
}
