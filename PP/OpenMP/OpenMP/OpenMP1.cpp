#include <iostream>
#include <omp.h>

using namespace std;
void OpenMP1()
{
    int n = 8;
#pragma omp parallel num_threads(n)
    {
        int threadCount = omp_get_num_threads();
        int threadNum = omp_get_thread_num();
        printf("%d %d Hello world\n", threadNum, threadCount);
    }
}