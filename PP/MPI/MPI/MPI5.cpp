#include <iostream>
#include "mpi.h"

using namespace std;
void MPI5(int argc, char** argv)
{
	int rank, size;
	MPI_Init(&argc, &argv);
	MPI_Comm_size(MPI_COMM_WORLD, &size);
	MPI_Comm_rank(MPI_COMM_WORLD, &rank);

	const int n = 30;
	int arrA[n];
	int arrB[n];
	if (rank == 0)
	{
		srand(time(NULL));
		for (int i = 0; i < n; i++)
		{
			arrA[i] = rand() % 20 - 10;
			arrB[i] = rand() % 20 - 10;
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

	auto localArrA = new int[sendCounts[rank]];
	auto localArrB = new int[sendCounts[rank]];
	MPI_Scatterv(&arrA[0], sendCounts, disps, MPI_INT, &localArrA[0], sendCounts[rank], MPI_INT, 0, MPI_COMM_WORLD);
	MPI_Scatterv(&arrB[0], sendCounts, disps, MPI_INT, &localArrB[0], sendCounts[rank], MPI_INT, 0, MPI_COMM_WORLD);

	int localSum = 0;
	for (int i = 0; i < sendCounts[rank]; i++)
	{
		localSum += localArrA[i] * localArrB[i];
	}

	printf("Process %d finished localSum=%d and localLength=%d\n", rank, localSum, sendCounts[rank]);
	cout << flush;
	MPI_Barrier(MPI_COMM_WORLD);

	int scalarProduct;
	MPI_Reduce(&localSum, &scalarProduct, 1, MPI_INT, MPI_SUM, 0, MPI_COMM_WORLD);
	if (rank == 0)
		printf("Scalar product = %d\n", scalarProduct);

	MPI_Finalize();
}