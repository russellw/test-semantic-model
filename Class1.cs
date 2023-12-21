class Class1 {
	void Method1() {
		Environment.Exit(0);
	}

	int Method2(int a) {
		var b = a + 5;
		var c = a + b;
		return b * c;
	}

	string Method3(int a) {
		return string.Format("{0}", a) + a;
	}

	string Method4(int a) {
		return a.ToString() + a;
	}

	int Method5() {
		return Method2(1) + Method2(2);
	}
}
