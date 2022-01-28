func foo(int a, int b)->int{
	return a + b;
}
for a = 0 to 10 {
	println(%d, a);
}
int r;
r = foo(1, 2);
print(%s, "sum of 1 and 2\n");
print(%d, r);