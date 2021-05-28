#include <iostream>
#include <omp.h>

using namespace std;

/*int max(int r, int n) 
{
	if (n > r)
		return n;
	else
		return r;
}*/

void OpenMP10()
{
	const int n = 6;
	const int m = 8;
	int d[n][m];
	srand(time(NULL));
	for (int i = 0; i < n; i++)
		for (int j = 0; j < m; j++)
			d[i][j] = rand() % 100;

	int min = d[0][0];
	int max = d[0][0];

//#pragma omp declare reduction (rwz:int:omp_out=max(omp_out,omp_in)) initializer(omp_priv=INT_MIN)

#pragma omp parallel for
	for (int i = 0; i < n; i++)
		for (int j = 0; j < m; j++)
		{
#pragma omp critical
			if (d[i][j] < min)
				min = d[i][j];
			if (d[i][j] > max)
				max = d[i][j];
		}
	printf("min=%d max=%d", min, max);
}