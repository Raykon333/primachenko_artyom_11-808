#include <iostream>
#include "mpi.h"
#include <algorithm>

using namespace std;
void MPI2(int argc, char** argv)
{
	int rank, size;
	MPI_Init(&argc, &argv);
	MPI_Comm_size(MPI_COMM_WORLD, &size);
	MPI_Comm_rank(MPI_COMM_WORLD, &rank);

	const int n = 30;
	int arr[n];
	if (rank == 0)
	{
		srand(time(NULL));
		for (int i = 0; i < n; i++)
		{
			arr[i] = rand() % 100;
		}
	}

	auto sendCounts = new int[size];
	auto disps = new int[size];
	disps[0] = 0;
	for (int i = 0; i < size; i++)
	{
		sendCounts[i] = n / size;
		if (i == size - 1)
			sendCounts[i] += n % size;
		if (i != 0)
			disps[i] = disps[i - 1] + sendCounts[i-1];
	}

	auto localArr = new int[sendCounts[rank]];
	MPI_Scatterv(&arr[0], sendCounts, disps, MPI_INT, &localArr[0], sendCounts[rank], MPI_INT, 0, MPI_COMM_WORLD);

	int localMax = *max_element(&localArr[0], &localArr[sendCounts[rank] - 1]);
	printf("Process %d, localMax = %d\n", rank, localMax);
	
	cout << flush;
	MPI_Barrier(MPI_COMM_WORLD);

	int max;
	MPI_Reduce(&localMax, &max, 1, MPI_INT, MPI_MAX, 0, MPI_COMM_WORLD);
	if (rank == 0)
		printf("Max = %d\n", max);

	MPI_Finalize();
}