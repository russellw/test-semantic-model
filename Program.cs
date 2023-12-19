using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using static System.Net.Mime.MediaTypeNames;

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
	}
}
