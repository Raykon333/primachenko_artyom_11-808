#include <iostream>
#include "mpi.h"

void MPI1(int argc, char** argv)
{
	int rank, size;
	MPI_Init(&argc, &argv);
	MPI_Comm_size(MPI_COMM_WORLD, &size);
	MPI_Comm_rank(MPI_COMM_WORLD, &rank);
	printf("Hello world (process %d, total %d)", rank, size);
	MPI_Finalize();
}