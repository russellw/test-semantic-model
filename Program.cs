using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class Program {
	static void Main(string[] _) {
		var compilation =
			CSharpCompilation.Create(null).AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
		// MetadataReference.CreateFromFile("bin\\Debug\\net7.0\\test-semantic-model.dll"));

		var file = "StaticClass.cs";
		var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file), CSharpParseOptions.Default, file);
		if (tree.GetDiagnostics().Any()) {
			foreach (var diagnostic in tree.GetDiagnostics())
				Console.Error.WriteLine(diagnostic);
			Environment.Exit(1);
		}
		compilation = compilation.AddSyntaxTrees(tree);

		file = "Class1.cs";
		tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file), CSharpParseOptions.Default, file);
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

	public override void VisitBinaryExpression(BinaryExpressionSyntax node) {
		base.VisitBinaryExpression(node);

		Console.WriteLine("----------------- type info");
		Console.WriteLine(node);
		Console.WriteLine(model.GetTypeInfo(node).Type);
		Console.WriteLine();
	}

	public override void VisitInvocationExpression(InvocationExpressionSyntax node) {
		base.VisitInvocationExpression(node);

		Console.WriteLine("----------------- symbol info");
		var symbolInfo = model.GetSymbolInfo(node);
		Console.WriteLine(node);
		Console.WriteLine(symbolInfo.Symbol);
		Console.WriteLine(symbolInfo.CandidateSymbols.ToArray());
		Console.WriteLine(symbolInfo.CandidateReason);
		Console.WriteLine();
	}

	readonly SemanticModel model;
}
