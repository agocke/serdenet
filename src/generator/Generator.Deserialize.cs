
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Serde.Diagnostics;

namespace Serde
{
    partial class Generator
    {
        private const string GeneratedVisitorName = "SerdeVisitor";

        private static (MemberDeclarationSyntax[], BaseListSyntax) GenerateDeserializeImpl(
            GeneratorExecutionContext context,
            ITypeSymbol receiverType,
            ExpressionSyntax receiverExpr)
        {
            TypeSyntax typeSyntax = ParseTypeName(receiverType.ToDisplayString());

            // `Serde.IDeserialize<'typeName'>
            var interfaceSyntax = QualifiedName(IdentifierName("Serde"), GenericName(
                Identifier("IDeserialize"),
                TypeArgumentList(SeparatedList(new[] { typeSyntax }))
            ));

            // Generate members for ISerialize.Deserialize implementation
            var method = GenerateDeserializeMethod(interfaceSyntax, receiverType);
            var visitorType = GenerateVisitor(receiverType, typeSyntax, context);
            var members = new MemberDeclarationSyntax[] { method, visitorType };
            var baseList = BaseList(SeparatedList(new BaseTypeSyntax[] { SimpleBaseType(interfaceSyntax) }));
            return (members, baseList);
        }

        // Generate method `void ISerialize.Deserialize(IDeserializer deserializer) { ... }`
        private static MethodDeclarationSyntax GenerateDeserializeMethod(
            QualifiedNameSyntax interfaceSyntax,
            ITypeSymbol typeSymbol)
        {
            var stmts = new List<StatementSyntax>();

            // var visitor = new 'GeneratedVisitorName'();
            stmts.Add(LocalDeclarationStatement(
                VariableDeclaration(
                    IdentifierName("var"),
                    SeparatedList(new[] { VariableDeclarator(
                        Identifier("visitor"),
                        argumentList: null,
                        initializer: EqualsValueClause(ObjectCreationExpression(
                            IdentifierName(GeneratedVisitorName),
                            ArgumentList(),
                            initializer: null)))
                    })
                )));


            // Generate statements:
            // var visitor = new 'GeneratedVisitorName'();

            // Three options:
            // 1. Built-in type
            // 2. Enum type
            // 3. Custom type
            //
            // var fieldNames = new[] { 'field1', 'field2', 'field3' ... };
            // return deserializer.DeserializeType<'TypeName', 'GeneratedVisitorName'>('TypeName', fieldNames, visitor);

            var serdeName = SerdeBuiltInName(typeSymbol.SpecialType);
            var typeSyntax = ParseTypeName(typeSymbol.ToString());
            if (serdeName is not null)
            {
                // Built-in type
                stmts.Add(ReturnStatement(InvocationExpression(
                    QualifiedName(
                        IdentifierName("deserializer"),
                        GenericName(
                            Identifier("Deserialize" + serdeName),
                            TypeArgumentList(SeparatedList(new TypeSyntax[] {
                                typeSyntax,
                                IdentifierName(GeneratedVisitorName)
                            }))
                        )
                    ),
                    ArgumentList(SeparatedList(new[] {
                        Argument(IdentifierName("visitor"))
                    }))
                )));
            }
            else if (typeSymbol.TypeKind == TypeKind.Enum)
            {
                // 2. Enum type
                stmts.Add(ReturnStatement(InvocationExpression(
                    QualifiedName(
                        IdentifierName("deserializer"),
                        GenericName(
                            Identifier("DeserializeString"),
                            TypeArgumentList(SeparatedList(new TypeSyntax[] {
                                typeSyntax,
                                IdentifierName(GeneratedVisitorName)
                            }))
                        )
                    ),
                    ArgumentList(SeparatedList(new[] {
                        Argument(IdentifierName("visitor"))
                    }))
                )));
            }
            else
            {
                // 3. Custom type
                var members = SymbolUtilities.GetPublicDataMembers(typeSymbol);
                var namesArray = ImplicitArrayCreationExpression(InitializerExpression(
                    SyntaxKind.ArrayInitializerExpression,
                    SeparatedList(members.Select(d => (ExpressionSyntax)StringLiteral(d.Name)))));

                // var fieldNames = new[] { 'field1', 'field2', 'field3' ... };
                stmts.Add(LocalDeclarationStatement(
                    VariableDeclaration(
                        IdentifierName("var"),
                        SeparatedList(new[] { VariableDeclarator(
                        Identifier("fieldNames"),
                        argumentList: null,
                        initializer: EqualsValueClause(namesArray)) })
                    )
                ));

                // return deserializer.DeserializeType<'TypeName', 'GeneratedVisitorName'>('TypeName', fieldNames, visitor);
                stmts.Add(ReturnStatement(InvocationExpression(
                    QualifiedName(
                        IdentifierName("deserializer"),
                        GenericName(
                            Identifier("DeserializeType"),
                            TypeArgumentList(SeparatedList(new TypeSyntax[] {
                            typeSyntax,
                            IdentifierName(GeneratedVisitorName)
                            })))
                        ),
                    ArgumentList(SeparatedList(new[] {
                    Argument(StringLiteral(typeSymbol.Name)),
                    Argument(IdentifierName("fieldNames")),
                    Argument(IdentifierName("visitor"))
                    }))
                )));
            }

            return MethodDeclaration(
                attributeLists: default,
                modifiers: TokenList(Token(SyntaxKind.StaticKeyword)),
                typeSyntax,
                explicitInterfaceSpecifier: ExplicitInterfaceSpecifier(interfaceSyntax),
                identifier: Identifier("Deserialize"),
                typeParameterList: TypeParameterList(SeparatedList(new[] {
                        TypeParameter("D"),
                    })),
                parameterList: ParameterList(SeparatedList(new[] { Parameter("D", "deserializer", byRef: true) })),
                constraintClauses: default,
                body: Block(stmts.ToArray()),
                expressionBody: null
                );
        }

        private static TypeDeclarationSyntax GenerateVisitor(ITypeSymbol type, TypeSyntax typeSyntax, GeneratorExecutionContext context)
        {
            // Serde.IDeserializeVisitor<'typeName'>
            var interfaceSyntax = QualifiedName(IdentifierName("Serde"), GenericName(
                Identifier("IDeserializeVisitor"),
                TypeArgumentList(SeparatedList(new[] { typeSyntax }))
            ));

            var typeName = typeSyntax.ToString();
            var typeMembers = new List<MemberDeclarationSyntax>();
            // Members:
            //     public string ExpectedTypeName => 'typeName';
            typeMembers.Add(PropertyDeclaration(
                attributeLists: default,
                modifiers: new SyntaxTokenList(new[] { Token(SyntaxKind.PublicKeyword) }),
                type: PredefinedType(Token(SyntaxKind.StringKeyword)),
                explicitInterfaceSpecifier: null,
                identifier: Identifier("ExpectedTypeName"),
                accessorList: null,
                expressionBody: ArrowExpressionClause(StringLiteral(typeName)),
                initializer: null,
                semicolonToken: Token(SyntaxKind.SemicolonToken)));

            // Three cases
            // 1. Built-in type
            // 2. Enum type
            // 3. Custom type

            var serdeName = SerdeBuiltInName(type.SpecialType);
            if (serdeName is not null)
            {
                var methodText = $"{typeName} IDeserializeVisitor<{typeName}>.Visit{serdeName}({typeName} x) => x;";
                typeMembers.Add(ParseMemberDeclaration(methodText)!);
            }
            else if (type.TypeKind == TypeKind.Enum)
            {
                var cases = new StringBuilder();
                foreach (var m in SymbolUtilities.GetPublicDataMembers(type))
                {
                    cases.Append(@$"
        case ""{m.Name}"":
            enumValue = {typeName}.{m.Name};
            break;");
                }
                var methodText = @$"
{typeName} Serde.IDeserializeVisitor<{typeName}>.VisitString(string s)
{{
    {typeName} enumValue;
    switch (s)
    {{
{cases}
        default:
            throw new InvalidDeserializeValueException(""Unexpected enum field name: "" + s);
    }}
    return enumValue;
}}";
                typeMembers.Add(ParseMemberDeclaration(methodText)!);
            }
            else
            {
                GenerateCustomTypeVisitor(type, context, typeName, typeMembers);
            }

            // private sealed class SerdeVisitor : IDeserializeVisitor<'typeName'>
            // {
            //     'Members'
            // }
            return ClassDeclaration(
                attributeLists: default,
                modifiers: new SyntaxTokenList(
                    Token(SyntaxKind.PrivateKeyword),
                    Token(SyntaxKind.SealedKeyword)),
                Identifier(GeneratedVisitorName),
                typeParameterList: null,
                baseList: BaseList(SeparatedList(new BaseTypeSyntax[] { SimpleBaseType(interfaceSyntax) })),
                constraintClauses: default,
                members: List(typeMembers));
        }

        private static void GenerateCustomTypeVisitor(ITypeSymbol type, GeneratorExecutionContext context, string typeName, List<MemberDeclarationSyntax> typeMembers)
        {
            var cases = new StringBuilder();
            var locals = new StringBuilder();
            var members = SymbolUtilities.GetPublicDataMembers(type);
            foreach (var m in members)
            {
                string wrapperName;
                var memberType = m.Type.WithNullableAnnotation(m.NullableAnnotation).ToDisplayString();
                if (ImplementsSerde(m.Type, context, SerdeUsage.Deserialize))
                {
                    wrapperName = memberType;
                }
                else if (TryGetPrimitiveWrapper(m.Type, SerdeUsage.Deserialize) is { } primWrap)
                {
                    wrapperName = primWrap;
                }
                else if (TryGetCompoundWrapper(m.Type, context, SerdeUsage.Deserialize) is { } compound)
                {
                    wrapperName = compound.ToString();
                }
                else if (TryCreateWrapper(m.Type, m, context, SerdeUsage.Deserialize) is { } wrap)
                {
                    wrapperName = wrap.ToString();
                }
                else
                {
                    // No built-in handling and doesn't implement ISerializable, error
                    context.ReportDiagnostic(CreateDiagnostic(
                        DiagId.ERR_DoesntImplementInterface,
                        m.Locations[0],
                        m.Symbol,
                        memberType,
                        "Serde.IDeserializable"));
                    wrapperName = memberType;
                }
                var localName = GetLocalName(m);
                locals.AppendLine($"Serde.Option<{memberType}> {localName} = default;");
                // Note, don't use nullable annotation as wrappers don't necessarily allow null
                cases.AppendLine(@$"
            case ""{m.GetFormattedName()}"":
                {localName} = d.GetNextValue<{memberType}, {wrapperName}>();
                break;");
            }

            string unknownMemberBehavior = SymbolUtilities.GetTypeOptions(type).DenyUnknownMembers
                ? $"throw new InvalidDeserializeValueException(\"Unexpected field or property name in type {typeName}: '\" + key + \"'\");"
                : "break;";

            string typeCreationExpr = GenerateTypeCreation(context, typeName, type, members);
            var methodText = $$"""
{{typeName}} Serde.IDeserializeVisitor<{{typeName}}>.VisitDictionary<D>(ref D d)
{
    {{locals}}
    while (d.TryGetNextKey<string, StringWrap>(out string? key))
    {
        switch (key)
        {
{{cases}}
            default:
                {{unknownMemberBehavior}}
        }
    }
    {{typeCreationExpr}}
    return newType;
}
""";
            typeMembers.Add(ParseMemberDeclaration(methodText)!);
        }

        /// <summary>
        /// If the type has a parameterless constructor then we will use that and just set
        /// each member in the initializer. If there is no parameterlss constructor, there
        /// must be a constructor signature as specified by the ConstructorSignature property
        /// in the SerdeTypeOptions.
        /// </summary>
        private static string GenerateTypeCreation(GeneratorExecutionContext context, string typeName, ITypeSymbol type, List<DataMemberSymbol> members)
        {
            var targetSignature = SymbolUtilities.GetTypeOptions(type).ConstructorSignature;
            var targetTuple = targetSignature as INamedTypeSymbol;
            var ctors = type.GetMembers(".ctor");
            IMethodSymbol? targetCtor = null;
            IMethodSymbol? parameterLessCtor = null;
            foreach (var ctorSymbol in ctors)
            {
                if (ctorSymbol is IMethodSymbol ctorMethod)
                {
                    if (targetTuple is not null && ctorMethod.Parameters.Length == targetTuple.TupleElements.Length)
                    {
                        bool mismatch = false;
                        for(int i = 0; i < targetTuple.TupleElements.Length; i++)
                        {
                            var elem = targetTuple.TupleElements[i];
                            var param = ctorMethod.Parameters[i];
                            if (!elem.Type.Equals(param.Type, SymbolEqualityComparer.Default))
                            {
                                mismatch = true;
                                break;
                            }
                        }
                        if (!mismatch)
                        {
                            targetCtor = ctorMethod;
                            break;
                        }
                    }
                    if (ctorMethod is { Parameters.Length: 0 })
                    {
                        parameterLessCtor = ctorMethod;
                        if (targetSignature is null)
                        {
                            break;
                        }
                    }
                }
            }
            if (targetSignature is not null && targetCtor is null)
            {
                context.ReportDiagnostic(CreateDiagnostic(DiagId.ERR_CantFindConstructorSignature, type.Locations[0]));
                return "";
            }

            var assignmentMembers = new List<DataMemberSymbol>(members);
            var assignments = new StringBuilder();
            var parameters = new StringBuilder();
            if (targetCtor is not null)
            {
                foreach (var p in targetCtor.Parameters)
                {
                    var index = assignmentMembers.FindIndex(m => m.Name == p.Name);
                    if (parameters.Length != 0)
                    {
                        parameters.Append(", ");
                    }
                    parameters.Append(GetMemberAccess(assignmentMembers[index]));
                    assignmentMembers.RemoveAt(index);
                }
            }

            foreach (var m in assignmentMembers)
            {
                assignments.AppendLine($"{m.Name} = {GetMemberAccess(m)},");
            }

            return $$"""
    var newType = new {{typeName}}({{parameters}}) {
        {{assignments}}
    };
""";
            static string GetMemberAccess(DataMemberSymbol m)
            {
                var localName = GetLocalName(m);
                return m.NullIfMissing
                    ? $"{localName}.GetValueOrDefault(null)"
                    : $"{localName}.GetValueOrThrow(\"{m.Name}\")";
            }
        }

        private static string GetLocalName(DataMemberSymbol m) => m.Name.ToLower();
    }
}