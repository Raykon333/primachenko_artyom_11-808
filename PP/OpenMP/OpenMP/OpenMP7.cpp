#include <iostream>
#include <omp.h>

using namespace std;
void OpenMP7()
{
    const int n = 12;
    int a[n], b[n], c[n];

#pragma omp parallel for num_threads(3) schedule(static, 2)
    for (int i = 0; i < n; i++)
    {
        a[i] = i;
        b[i] = 2 * (i + 1);
        printf("thread %d, total %d, a[%d]=%d b[%d]=%d\n",
            omp_get_thread_num(), omp_get_num_threads(), i, a[i], i, b[i]);
    }

#pragma omp parallel for num_threads(4) schedule(dynamic, 2)
    for (int i = 0; i < n; i++)
    {
        c[i] = a[i] + b[i];
        printf("thread %d, total %d, c[%d]=%d\n",
            omp_get_thread_num(), omp_get_num_threads(), i, c[i]);
    }
}