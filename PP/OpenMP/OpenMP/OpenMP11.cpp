#include <iostream>
#include <omp.h>

using namespace std;
void OpenMP11()
{
	const int n = 30;
	int a[n];
	srand(time(NULL));
	for (int i = 0; i < n; i++)
		a[i] = rand() % 100;

	int count = 0;
#pragma omp parallel for
	for (int i = 0; i < n; i++)
	{
		if (a[i] % 9 == 0)
#pragma omp atomic
			count++;
	}

	printf("count=%d", count);
}