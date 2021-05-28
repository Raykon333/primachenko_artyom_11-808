#include <iostream>
#include <omp.h>

using namespace std;

void OpenMP8()
{
    const int n = 16000;
    auto a = new int[n];
    auto b = new double[n - 2];
    double tt;
    for (int i = 0; i < n; i++)
        a[i] = i;

    tt = omp_get_wtime();
#pragma omp parallel for schedule(static, 10)
    for (int i = 0; i < n - 2; i++)
        b[i] = (a[i] + a[i + 1] + a[i + 2]) / 3.0;
    tt = omp_get_wtime() - tt;
    printf("Static with chunk 10: %f\n", tt);

    tt = omp_get_wtime();
#pragma omp parallel for schedule(static, 100)
    for (int i = 0; i < n - 2; i++)
        b[i] = (a[i] + a[i + 1] + a[i + 2]) / 3.0;
    tt = omp_get_wtime() - tt;
    printf("Static with chunk 100: %f\n", tt);

    tt = omp_get_wtime();
#pragma omp parallel for schedule(dynamic, 10)
    for (int i = 0; i < n - 2; i++)
        b[i] = (a[i] + a[i + 1] + a[i + 2]) / 3.0;
    tt = omp_get_wtime() - tt;
    printf("Dynamic with chunk 10: %f\n", tt);

    tt = omp_get_wtime();
#pragma omp parallel for schedule(dynamic, 100)
    for (int i = 0; i < n - 2; i++)
        b[i] = (a[i] + a[i + 1] + a[i + 2]) / 3.0;
    tt = omp_get_wtime() - tt;
    printf("Dynamic with chunk 100: %f\n", tt);

    tt = omp_get_wtime();
#pragma omp parallel for schedule(guided, 10)
    for (int i = 0; i < n - 2; i++)
        b[i] = (a[i] + a[i + 1] + a[i + 2]) / 3.0;
    tt = omp_get_wtime() - tt;
    printf("Guided with chunk 10: %f\n", tt);

    tt = omp_get_wtime();
#pragma omp parallel for schedule(guided, 100)
    for (int i = 0; i < n - 2; i++)
        b[i] = (a[i] + a[i + 1] + a[i + 2]) / 3.0;
    tt = omp_get_wtime() - tt;
    printf("Guided with chunk 100: %f\n", tt);

    tt = omp_get_wtime();
#pragma omp parallel for schedule(guided, 1000)
    for (int i = 0; i < n - 2; i++)
        b[i] = (a[i] + a[i + 1] + a[i + 2]) / 3.0;
    tt = omp_get_wtime() - tt;
    printf("Guided with chunk 1000: %f\n", tt);

    tt = omp_get_wtime();
#pragma omp parallel for schedule(runtime)
    for (int i = 0; i < n - 2; i++)
        b[i] = (a[i] + a[i + 1] + a[i + 2]) / 3.0;
    tt = omp_get_wtime() - tt;
    printf("Auto: %f\n", tt);
}