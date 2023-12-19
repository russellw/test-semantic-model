﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class Program {
	static void Main(string[] _) {
		var compilation =
			CSharpCompilation.Create(null).AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

		var file = "Class1.cs";
		var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file), CSharpParseOptions.Default, file);
		if (tree.GetDiagnostics().Any()) {
			foreach (var diagnostic in tree.GetDiagnostics())
				Console.Error.WriteLine(diagnostic);
			Environment.Exit(1);
		}
		compilation = compilation.AddSyntaxTrees(tree);

		var model = compilation.GetSemanticModel(tree);
		var root = tree.GetCompilationUnitRoot();
		new Walker(model).Visit(root);
	}
}

class Walker: CSharpSyntaxWalker {
	public Walker(SemanticModel model) {
		this.model = model;
	}

	public override void VisitInvocationExpression(InvocationExpressionSyntax node) {
		base.VisitInvocationExpression(node);

		var symbolInfo = model.GetSymbolInfo(node);
		Console.WriteLine(node);
		Console.WriteLine(symbolInfo.Symbol);
		Console.WriteLine(symbolInfo.CandidateSymbols.ToArray());
	}

	readonly SemanticModel model;
}
