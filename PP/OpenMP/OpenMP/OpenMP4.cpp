#include <iostream>
#include <omp.h>
#include <algorithm>

using namespace std;
void OpenMP4()
{
    const int n = 10;
    int a[n], b[n];
    for (int i = 0; i < n; i++)
    {
        a[i] = (5 - i) * (5 - i) * (8 - i);
        b[i] = i + a[i];
    }

    int minA = 0;
    int maxB = 0;
#pragma omp parallel num_threads(2)
    {
        int threadNum = omp_get_thread_num();
        if (threadNum == 0)
            minA = *min_element(a, a + n);
        if (threadNum == 1)
            maxB = *max_element(b, b + n);
    }
    printf("%d %d", minA, maxB);
}