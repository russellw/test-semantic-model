using Microsoft.CodeAnalysis;

public static class StaticClass {
	public static int Cube(int a) {
		return Square(a) * a;
	}

	public static int Square(int a) {
		return a * a;
	}

	public static SyntaxNode Ultima1(SyntaxNode node) {
		return Ultima2(node);
	}

	public static SyntaxNode Ultima2(SyntaxNode node) {
		return node;
	}

	public static string Xyzzy1(System.Text.StringBuilder sb) {
		return Xyzzy2(sb);
	}

	public static string Xyzzy2(System.Text.StringBuilder sb) {
		return sb.ToString();
	}
}
