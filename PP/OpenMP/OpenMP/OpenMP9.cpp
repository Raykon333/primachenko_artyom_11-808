#include <iostream>
#include <omp.h>
#include <chrono>

using namespace std;
using namespace chrono;
void OpenMP9()
{
	const int n = 20000, m = 18000;
	auto matrix = new int[n][m];
	auto vector = new int[m];
	auto resultVector = new int[n];

	for (int j = 0; j < m; j++)
	{
		vector[j] = j;
		for (int i = 0; i < n; i++)
			matrix[i][j] = i * j;
	}
	for (int i = 0; i < n; i++)
		resultVector[i] = i;

	printf("Start\n");

	steady_clock::time_point begin;
	steady_clock::time_point end;

	begin = chrono::steady_clock::now();
	for (int i = 0; i < n; i++)
	{
		int sum = 0;
		for (int j = 0; j < m; j++)
		{
			sum += matrix[i][j] * vector[j];
		}
		resultVector[i] = sum;
	}

	end = steady_clock::now();
	cout << duration_cast<microseconds> (end - begin).count() << "\n";


	begin = steady_clock::now();

#pragma omp parallel for schedule(guided, n/100)
	for (int i = 0; i < n; i++) 
	{
		int sum = 0;
#pragma omp parallel for schedule(guided, m/100) reduction(+: sum)
		for (int j = 0; j < m; j++)
			sum += matrix[i][j] * vector[j];
		resultVector[i] = sum;
	}

	end = steady_clock::now();
	cout << duration_cast<microseconds> (end - begin).count() << "\n";
}