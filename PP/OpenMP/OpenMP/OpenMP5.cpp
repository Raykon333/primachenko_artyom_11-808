#include <iostream>
#include <omp.h>
#include <algorithm>

using namespace std;
void OpenMP5()
{
	const int n = 6;
	const int m = 8;
	int d[n][m];
	srand(time(NULL));
	for (int i = 0; i < n; i++)
		for (int j = 0; j < m; j++)
			d[i][j] = rand() % 100;

#pragma omp parallel 
	{
#pragma omp sections
		{
#pragma omp section
			{
				int sum = 0;
				for (int i = 0; i < n; i++)
					for (int j = 0; j < m; j++)
						sum += d[i][j];
				printf("thread %d, average=%f\n", omp_get_thread_num(), (float)sum / n / m);
			}

#pragma omp section
			{
				int min = *min_element(d[0], d[0] + m);
				for (int i = 1; i < n; i++)
					for (int j = 0; j < m; j++)
						if (d[i][j] < min)
							min = d[i][j]; 
				printf("thread %d, minimum=%d\n", omp_get_thread_num(), min);
			}

#pragma omp section
			{
				int count = 0;
				for (int i = 0; i < n; i++)
					for (int j = 0; j < m; j++)
						if (d[i][j] % 3 == 0)
							count++;
				printf("thread %d, count=%d\n", omp_get_thread_num(), count);
			}
		}
	}
}