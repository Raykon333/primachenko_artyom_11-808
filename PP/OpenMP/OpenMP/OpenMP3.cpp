#include <iostream>
#include <omp.h>

using namespace std;
void OpenMP3()
{
	int a = 1;
	int b = 2;

	printf("Before first region: a=%d b=%d\n", a, b);
#pragma omp parallel num_threads(2) private(a) firstprivate(b)
	{
		int a = 1;
		int threadNum = omp_get_thread_num();
#pragma omp critical
		{
			a += threadNum;
			b += threadNum;
		}
		printf("In thread number %d: a=%d b=%d\n", threadNum, a, b);
	}
	printf("After first region: a=%d b=%d\n\n", a, b);

	printf("Before second region: a=%d b=%d\n", a, b);
#pragma omp parallel num_threads(4) shared(a) private(b)
	{
		int b = 2;
		int threadNum = omp_get_thread_num();
#pragma omp critical
		{
			a -= threadNum;
			b -= threadNum;
		}
		printf("In thread number %d: a=%d b=%d\n", threadNum, a, b);
	}
	printf("After second region: a=%d b=%d\n\n", a, b);
}