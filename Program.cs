using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

static class Program {
	delegate void Callback(string file);

	static SemanticModel model = null!;

	static void Descend(string path, Callback f) {
		if (Directory.Exists(path)) {
			foreach (var entry in new DirectoryInfo(path).EnumerateFileSystemInfos()) {
				if (entry is DirectoryInfo) {
					switch (entry.Name) {
					case "bin":
					case "obj":
						continue;
					}
					if (entry.Name.StartsWith('.'))
						continue;
				}
				Descend(entry.FullName, f);
			}
			return;
		}
		f(path);
	}

	static void Help() {
		var name = typeof(Program).Assembly.GetName().Name;
		Console.WriteLine($"{name} [options] file...");
		Console.WriteLine("");
		Console.WriteLine("-h  Show help");
		Console.WriteLine("-V  Show version");
	}

	static void Indent(int n) {
		while (0 != n--)
			Console.Write("  ");
	}

	static void Main(string[] args) {
		try {
			// Command line
			var paths = new List<string>();
			foreach (var arg in args) {
				var s = arg;
				if (!s.StartsWith('-')) {
					paths.Add(s);
					continue;
				}
				while (s.StartsWith('-'))
					s = s[1..];
				switch (s) {
				case "?":
				case "h":
				case "help":
					Help();
					return;
				case "V":
				case "v":
				case "version":
					Version();
					return;
				default:
					throw new IOException(arg + ": unknown option");
				}
			}
			if (!paths.Any())
				paths.Add(".");

			// Source files
			var compilation = CSharpCompilation.Create(null);
			var trees = new List<SyntaxTree>();
			foreach (var path in paths)
				Descend(path, file => {
					if (!Path.GetExtension(file).Equals(".cs", StringComparison.OrdinalIgnoreCase))
						return;
					var text = File.ReadAllText(file);
					var tree = CSharpSyntaxTree.ParseText(text, CSharpParseOptions.Default, file);
					foreach (var diagnostic in tree.GetDiagnostics())
						Console.Error.WriteLine(diagnostic);
					compilation = compilation.AddSyntaxTrees(tree);
					trees.Add(tree);
				});

			// Output
			foreach (var tree in trees) {
				model = compilation.GetSemanticModel(tree);
				var root = tree.GetCompilationUnitRoot();
				Print(root);
			}
		} catch (IOException e) {
			Console.Error.WriteLine(e.Message);
			Environment.Exit(1);
		}
	}

	static void Print(SyntaxNode node, int level = 0) {
		Indent(level);
		Console.Write(node.Kind());
		Console.Write(' ');
		switch (node) {
		case BaseTypeDeclarationSyntax baseTypeDeclaration:
			Console.Write(baseTypeDeclaration.Identifier);
			Console.Write(baseTypeDeclaration.BaseList);
			break;
		case IdentifierNameSyntax identifierName:
			Console.Write(identifierName.Identifier);
			break;
		case InvocationExpressionSyntax: {
			Console.Write('|');
			var info = model.GetSymbolInfo(node);
			if (info.Symbol != null) {
				Console.Write(' ');
				Console.Write(info.Symbol);
			}
			if (info.CandidateSymbols.Any()) {
				Console.Write(' ');
				Console.Write(info.CandidateSymbols.ToArray());
			}
			if (CandidateReason.None != info.CandidateReason) {
				Console.Write(' ');
				Console.Write(info.CandidateReason);
			}
			break;
		}
		case MethodDeclarationSyntax methodDeclaration:
			Console.Write(methodDeclaration.Modifiers);
			Console.Write(' ');
			Console.Write(methodDeclaration.Identifier);
			Console.Write(methodDeclaration.ParameterList);
			Console.Write(" | ");
			Console.Write(model.GetDeclaredSymbol(methodDeclaration));
			break;
		case ParameterSyntax parameter:
			Console.Write(parameter.Identifier);
			break;
		case PredefinedTypeSyntax predefinedType:
			Console.Write(predefinedType.Keyword);
			break;
		}
		Console.WriteLine();
		foreach (var a in node.ChildNodes())
			Print(a, level + 1);
	}

	static void Version() {
		var name = typeof(Program).Assembly.GetName().Name;
		var version = typeof(Program).Assembly.GetName()?.Version?.ToString(2);
		Console.WriteLine($"{name} {version}");
	}
}
