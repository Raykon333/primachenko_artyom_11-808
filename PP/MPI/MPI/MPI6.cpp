#include <iostream>
#include "mpi.h"

using namespace std;
void MPI6(int argc, char** argv)
{
	int rank, size;
	MPI_Init(&argc, &argv);
	MPI_Comm_size(MPI_COMM_WORLD, &size);
	MPI_Comm_rank(MPI_COMM_WORLD, &rank);

	const int n = 9;
	const int m = 6;
	auto matrix = new int[n][m];
	if (rank == 0)
	{
		srand(time(NULL));
		for (int i = 0; i < n; i++)
		{
			for (int j = 0; j < m; j++)
			{
				matrix[i][j] = rand() % 100;
				printf("%d\t", matrix[i][j]);
			}
			cout << endl;
		}
	}

	auto sendCounts = new int[size];
	auto disps = new int[size];
	disps[0] = 0;
	for (int i = 0; i < size; i++)
	{
		sendCounts[i] = n / size * m;
		if (i == size - 1)
			sendCounts[i] += n % size * m;
		if (i != 0)
			disps[i] = disps[i - 1] + sendCounts[i - 1];
	}

	auto localArr = new int[sendCounts[rank]];
	MPI_Scatterv(&matrix[0][0], sendCounts, disps, MPI_INT, &localArr[0], sendCounts[rank], MPI_INT, 0, MPI_COMM_WORLD);

	int localMaxMin = INT_MIN;
	int localMinMax = INT_MAX;
	for (int i = 0; i < sendCounts[rank] / m; i++)
	{
		int lineMin = localArr[i * m];
		int lineMax = localArr[i * m];
		for (int j = 1; j < m; j++)
		{
			if (localArr[i * m + j] < lineMin)
				lineMin = localArr[i * m + j];
			if (localArr[i * m + j] > lineMax)
				lineMax = localArr[i * m + j];
		}
		printf("Process %d, line %d, min %d, max %d\n", rank, rank * n / size + i, lineMin, lineMax);
		if (lineMin > localMaxMin)
			localMaxMin = lineMin;
		if (lineMax < localMinMax)
			localMinMax = lineMax;
	}
	printf("Process %d, Local MaxMin %d, Local MinMax %d\n", rank, localMaxMin, localMinMax);

	cout << flush;

	int globalMaxMin, globalMinMax;
	MPI_Reduce(&localMaxMin, &globalMaxMin, 1, MPI_INT, MPI_MAX, 0, MPI_COMM_WORLD);
	MPI_Reduce(&localMinMax, &globalMinMax, 1, MPI_INT, MPI_MIN, 0, MPI_COMM_WORLD);
	if (rank == 0)
	{
		printf("Global MaxMin = %d, Global MinMax = %d\n", globalMaxMin, globalMinMax);
	}

	MPI_Finalize();
}