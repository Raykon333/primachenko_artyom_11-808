#include <iostream>
#include "mpi.h"
#include <math.h>

using namespace std;
void MPI3(int argc, char** argv)
{
	int rank, size;
	MPI_Init(&argc, &argv);
	MPI_Comm_size(MPI_COMM_WORLD, &size);
	MPI_Comm_rank(MPI_COMM_WORLD, &rank);

	int iterations = 0;
	if (rank == 0)
	{
		printf("Enter iterations count:\n");
		cin >> iterations;
	}
	MPI_Bcast(&iterations, 1, MPI_INT, 0, MPI_COMM_WORLD);

	int count = 0;
	int localIterations;
	localIterations = iterations / size;
	if (rank == size - 1)
		localIterations += iterations % size;
	srand((unsigned)time(NULL) ^ rank);
	for (int i = 0; i < localIterations; i++)
	{
		double rand1 = (double)rand() / RAND_MAX * 2 - 1;
		double rand2 = (double)rand() / RAND_MAX * 2 - 1;
		double dist = sqrt(rand1 * rand1 + rand2 * rand2);
		if (dist < 1)
			count++;
	}

	printf("Process %d finished with count %d of total %d\n", rank, count, localIterations);
	cout << flush;
	MPI_Barrier(MPI_COMM_WORLD);

	int counts;
	MPI_Reduce(&count, &counts, 1, MPI_INT, MPI_SUM, 0, MPI_COMM_WORLD);
	if (rank == 0)
		printf("pi estimation = %f\n", (double)counts / iterations * 4);

	MPI_Finalize();
}