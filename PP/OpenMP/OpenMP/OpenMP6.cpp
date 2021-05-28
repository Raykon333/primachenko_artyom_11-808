#include <iostream>
#include <omp.h>

using namespace std;
void OpenMP6()
{
	const int n = 100;
	int a[n];
	for (int i = 0; i < n; i++)
		a[i] = i;

	int sum = 0;
#pragma omp parallel for
	for (int i = 0; i < n; i++)
	{
		sum += a[i];
	}
	printf("Without reduction sum=%d\n", sum);

	sum = 0;
#pragma omp parallel for reduction(+: sum)
	for (int i = 0; i < n; i++)
		sum += a[i];
	printf("With reduction sum=%d\n", sum);
}