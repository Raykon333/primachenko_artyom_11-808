#include <iostream>
#include <omp.h>

using namespace std;
void OpenMP12()
{
	const int n = 100;
	int a[n];
	srand(time(NULL));
	for (int i = 0; i < n; i++)
		a[i] = rand() % 10000;

	int max = numeric_limits<int>::min();
	bool foundOne = false;
#pragma omp parallel for
	for (int i = 0; i < n; i++)
	{
		if (a[i] % 7 == 0 && a[i] > max)
		{
			foundOne = true;
#pragma omp critical
			{
				max = a[i];
			}
		}
	}

	if (foundOne)
		printf("max=%d", max);
	else
		printf("No such numbers");
}