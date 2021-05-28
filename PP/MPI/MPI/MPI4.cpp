#include <iostream>
#include "mpi.h"

using namespace std;
void MPI4(int argc, char** argv)
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
			arr[i] = rand() % 200 - 100;
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
			disps[i] = disps[i - 1] + sendCounts[i - 1];
	}

	auto localArr = new int[sendCounts[rank]];
	MPI_Scatterv(&arr[0], sendCounts, disps, MPI_INT, &localArr[0], sendCounts[rank], MPI_INT, 0, MPI_COMM_WORLD);

	int vec[2] = { 0, 0 };
	for (int i = 0; i < sendCounts[rank]; i++)
	{
		if (localArr[i] > 0)
		{
			vec[0]++;
			vec[1] += localArr[i];
		}
	}

	printf("Process %d ended with count %d and sum %d\n", rank, vec[0], vec[1]);
	cout << flush;
	MPI_Barrier(MPI_COMM_WORLD);

	int resVec[2];
	MPI_Reduce(&vec, &resVec, 2, MPI_INT, MPI_SUM, 0, MPI_COMM_WORLD);

	if (rank == 0)
		printf("Result: count=%d, sum=%d, avg=%f\n", resVec[0], resVec[1], (double)resVec[1] / resVec[0]);

	MPI_Finalize();
}