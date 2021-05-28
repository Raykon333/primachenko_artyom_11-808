#include <iostream>
#include "mpi.h"
#include <algorithm>
#include <chrono>
#include <inttypes.h>

using namespace std;
using namespace chrono;
void MPI10(int argc, char** argv)
{
	int rank, size;
	MPI_Init(&argc, &argv);
	MPI_Comm_size(MPI_COMM_WORLD, &size);
	MPI_Comm_rank(MPI_COMM_WORLD, &rank);

	const int n = 1000000;
	auto arr = new int[n];
	if (rank == 0)
	{
		srand(time(NULL));
		for (int i = 0; i < n; i++)
		{
			arr[i] = rand() % 100;
		}
	}

	int bufsize = 4*n + 4*MPI_BSEND_OVERHEAD;
	int* buf = (int*)malloc(bufsize);
	MPI_Buffer_attach(buf, bufsize);

	if (rank == 0)
	{
		steady_clock::time_point begin;
		steady_clock::time_point end;
		MPI_Status status;

		begin = chrono::steady_clock::now();
		MPI_Send(&arr[0], n, MPI_INT, 1, 0, MPI_COMM_WORLD);
		MPI_Recv(&arr[0], n, MPI_INT, 1, 0, MPI_COMM_WORLD, &status);
		end = steady_clock::now();
		printf("Warmup Send: %" PRId64 "\n", duration_cast<microseconds>(end - begin).count());

		begin = chrono::steady_clock::now();
		MPI_Send(&arr[0], n, MPI_INT, 1, 1, MPI_COMM_WORLD);
		MPI_Recv(&arr[0], n, MPI_INT, 1, 2, MPI_COMM_WORLD, &status);
		end = steady_clock::now();
		printf("Send: %" PRId64 "\n", duration_cast<microseconds>(end - begin).count());

		begin = chrono::steady_clock::now();
		MPI_Ssend(&arr[0], n, MPI_INT, 1, 3, MPI_COMM_WORLD);
		MPI_Recv(&arr[0], n, MPI_INT, 1, 4, MPI_COMM_WORLD, &status);
		end = steady_clock::now();
		printf("Ssend: %" PRId64 "\n", duration_cast<microseconds>(end - begin).count());

		begin = chrono::steady_clock::now();
		MPI_Bsend(&arr[0], n, MPI_INT, 1, 5, MPI_COMM_WORLD);
		MPI_Recv(&arr[0], n, MPI_INT, 1, 6, MPI_COMM_WORLD, &status);
		end = steady_clock::now();
		printf("Bsend: %" PRId64 "\n", duration_cast<microseconds>(end - begin).count());

		begin = chrono::steady_clock::now();
		MPI_Rsend(&arr[0], n, MPI_INT, 1, 7, MPI_COMM_WORLD);
		MPI_Recv(&arr[0], n, MPI_INT, 1, 8, MPI_COMM_WORLD, &status);
		end = steady_clock::now();
		printf("Rsend: %" PRId64 "\n", duration_cast<microseconds>(end - begin).count());
	}
	else if (rank == 1)
	{
		MPI_Status status;

		MPI_Recv(&arr[0], n, MPI_INT, 0, 0, MPI_COMM_WORLD, &status);
		MPI_Send(&arr[0], n, MPI_INT, 0, 0, MPI_COMM_WORLD);

		MPI_Recv(&arr[0], n, MPI_INT, 0, 1, MPI_COMM_WORLD, &status);
		MPI_Send(&arr[0], n, MPI_INT, 0, 2, MPI_COMM_WORLD);

		MPI_Recv(&arr[0], n, MPI_INT, 0, 3, MPI_COMM_WORLD, &status);
		MPI_Ssend(&arr[0], n, MPI_INT, 0, 4, MPI_COMM_WORLD);

		MPI_Recv(&arr[0], n, MPI_INT, 0, 5, MPI_COMM_WORLD, &status);
		MPI_Bsend(&arr[0], n, MPI_INT, 0, 6, MPI_COMM_WORLD);

		MPI_Recv(&arr[0], n, MPI_INT, 0, 7, MPI_COMM_WORLD, &status);
		MPI_Rsend(&arr[0], n, MPI_INT, 0, 8, MPI_COMM_WORLD);
	}

	MPI_Buffer_detach(&buf, &bufsize);

	MPI_Finalize();
}