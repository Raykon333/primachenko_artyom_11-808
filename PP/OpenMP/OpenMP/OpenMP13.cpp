#include <iostream>
#include <omp.h>

using namespace std;
void Option1()
{
    int n = 8;
    int currentThread = n - 1;
#pragma omp parallel num_threads(n)
    {
        int threadCount = omp_get_num_threads();
        int threadNum = omp_get_thread_num();
        while (currentThread != threadNum);
        printf("%d %d Hello world\n", threadNum, threadCount);
        currentThread--;
    }
    printf("\n");
}

void Option2()
{
    int n = 8;
#pragma omp parallel num_threads(n)
    {
        int threadCount = omp_get_num_threads();
        int threadNum = omp_get_thread_num();
        for (int i = n - 1; i >= 0; i--)
        {
#pragma omp barrier
            {
                if (i == threadNum)
                {
                    printf("%d %d Hello world\n", threadNum, threadCount);
                }
            }
        }
    }
    printf("\n");
}

void OpenMP13()
{
    Option1();
    Option2();
}