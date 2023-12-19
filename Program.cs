using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class Program {
	static void Main(string[] _) {
		var compilation = CSharpCompilation.Create(null)
							  .WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication))
							  .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
											 MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
											 MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location),
											 MetadataReference.CreateFromFile(Path.Combine(
												 Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Runtime.dll")));

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
