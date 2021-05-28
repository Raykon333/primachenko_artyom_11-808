#include <iostream>
#include <omp.h>

using namespace std;

void ParallelIf(int threads)
{
#pragma omp parallel num_threads(threads) if (threads > 2)
	{
		int threadCount = omp_get_num_threads();
		int threadNum = omp_get_thread_num();
		printf("%d %d\n", threadNum, threadCount);
	}
	printf("\n");
}

void OpenMP2()
{
	ParallelIf(3);
	ParallelIf(2);
}