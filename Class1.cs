class Class1 {
	public Class1(int x) {
		this.x = x;
	}

	public int Square() {
		return StaticClass.Square(x);
	}

	int x;
}
